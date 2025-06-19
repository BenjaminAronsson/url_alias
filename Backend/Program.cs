using UrlAlias;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(new AliasService("aliases.json"));

var app = builder.Build();

app.MapAliasEndpoints();

app.Run();
