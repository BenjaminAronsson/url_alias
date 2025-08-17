using System.IO.Compression;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.FileProviders;
using UrlAlias.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddUrlAliasServices();

builder.Services.Configure<ForwardedHeadersOptions>(o =>
{
    o.ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor;
});

builder.Services.AddResponseCompression(o =>
{
    o.EnableForHttps = true;
    o.Providers.Add<BrotliCompressionProvider>();
    o.Providers.Add<GzipCompressionProvider>();
});
builder.Services.Configure<BrotliCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest);
builder.Services.Configure<GzipCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest);

var app = builder.Build();

app.UseResponseCompression();

//Uses global exception handler for unhandled exceptions
app.UseExceptionHandler(exceptionHandlerApp =>
    exceptionHandlerApp.Run(async context =>
        await Results.Problem(extensions: new Dictionary<string, object?>
        {
            ["traceId"] = context.TraceIdentifier,
        }).ExecuteAsync(context)));
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Path to Angular dist
var spaDist = Path.Combine(app.Environment.ContentRootPath, "ClientApp", "dist", "app");

// Default file + static files under /app
app.UseDefaultFiles(new DefaultFilesOptions
{
    RequestPath = "/app",
    DefaultFileNames = { "index.html" },
    FileProvider = new PhysicalFileProvider(spaDist)
});

app.UseStaticFiles(new StaticFileOptions
{
    RequestPath = "/app",
    FileProvider = new PhysicalFileProvider(spaDist),
    OnPrepareResponse = ctx =>
    {
        var path = ctx.File.PhysicalPath ?? string.Empty;
        if (Path.GetFileName(path).Equals("index.html", StringComparison.OrdinalIgnoreCase))
        {
            ctx.Context.Response.Headers.CacheControl = "no-cache, no-store, must-revalidate";
            ctx.Context.Response.Headers.Pragma = "no-cache";
            ctx.Context.Response.Headers.Expires = "0";
        }
        else
        {
            ctx.Context.Response.Headers.CacheControl = "public, max-age=31536000, immutable";
        }
    }
});

app.MapAliasEndpoints();

// SPA fallback for client-side routing under /app
app.MapFallback(async context =>
{
    if (context.Request.Path.StartsWithSegments("/app"))
    {
        var path = context.Request.Path.Value ?? "";
        var looksLikeFile = path.AsSpan().Contains('.'); // crude check for assets
        if (!looksLikeFile)
        {
            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.SendFileAsync(Path.Combine(spaDist, "index.html"));
            return;
        }
    }

    context.Response.StatusCode = StatusCodes.Status404NotFound;
});

await app.RunAsync();