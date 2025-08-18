using System.IO.Compression;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.FileProviders;
using UlrAlias.Backend.endpoints;
using UlrAlias.Backend.Extensions;
using Microsoft.EntityFrameworkCore;
using UlrAlias.Backend.Data;
using UlrAlias.Backend.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddUrlAliasServices(builder.Configuration);

var app = builder.Build();

app.UseResponseCompression();

//Uses global exception handler for unhandled exceptions
app.UseExceptionHandler(exceptionHandlerApp =>
    exceptionHandlerApp.Run(async context =>
        await Results.Problem(extensions: new Dictionary<string, object?>
        {
            ["traceId"] = context.TraceIdentifier
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

await SetupDb(app);

await app.RunAsync();
return;


#region Populate db
async Task SetupDb(WebApplication webApplication)
{
    using var scope = webApplication.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AliasDbContext>();

    // Apply migrations to ensure the database schema is created
    await dbContext.Database.MigrateAsync();

    if (!await dbContext.AliasEntries.AnyAsync())
    {
        var entries = Enumerable.Range(1, 1000).Select(i => new AliasEntry(
            $"alias{i}",
            $"https://google.com/?q={i}",
            DateTimeOffset.UtcNow.AddDays(2 + i)
        ));
        dbContext.AliasEntries.AddRange(entries);
        await dbContext.SaveChangesAsync();
    }
}
#endregion

