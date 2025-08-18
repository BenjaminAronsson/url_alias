namespace UlrAlias.Application.Services;

public interface IUrlShortener
{
    string GenerateAlias(string url);
}
