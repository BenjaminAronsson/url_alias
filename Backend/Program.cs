using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

const string DataFile = "aliases.json";

static IDictionary<string, string> LoadAliases()
{
    if (File.Exists(DataFile))
    {
        try
        {
            var json = File.ReadAllText(DataFile);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
        }
        catch
        {
            return new Dictionary<string, string>();
        }
    }
    return new Dictionary<string, string>();
}

static void SaveAliases(IDictionary<string, string> aliases)
{
    var json = JsonSerializer.Serialize(aliases, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(DataFile, json);
}

app.MapGet("/api/aliases", () => LoadAliases());

app.MapGet("/api/aliases/{alias}", (string alias) =>
{
    var aliases = LoadAliases();
    return aliases.TryGetValue(alias, out var url) ? Results.Ok(url) : Results.NotFound();
});

app.MapPost("/api/aliases", (AliasInput input) =>
{
    var aliases = LoadAliases();
    if (aliases.ContainsKey(input.Alias))
    {
        return Results.Conflict(new { message = "Alias already exists" });
    }
    aliases[input.Alias] = input.Url;
    SaveAliases(aliases);
    return Results.Created($"/api/aliases/{input.Alias}", input);
});

app.Run();

record AliasInput(string Alias, string Url);
