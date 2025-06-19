using Microsoft.AspNetCore.Builder;
using UrlAlias.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddUrlAliasServices();

var app = builder.Build();

app.MapAliasEndpoints();

app.Run();
