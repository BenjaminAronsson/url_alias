using Microsoft.Extensions.FileProviders;
using Ulr_Alias.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddUrlAliasServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "src", "dist")),
    RequestPath = "/app"
});

app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 404 &&
        context.Request.Path.StartsWithSegments("/app") &&
        !Path.HasExtension(context.Request.Path.Value))
    {
        context.Request.Path = "/app/index.html";
        context.Response.StatusCode = 200;
        await next();
    }
});

app.UseRouting();

app.UseHttpsRedirection();

app.MapAliasEndpoints();

app.Run();
