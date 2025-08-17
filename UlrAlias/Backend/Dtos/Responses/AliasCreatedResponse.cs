using UlrAlias.Backend.DTos;

namespace UlrAlias.Backend.Dtos.Responses;

public class AliasCreatedResponse: AliasEntryDto
{
    public string ShortUrl { get; private init; }

    public AliasCreatedResponse(AliasEntryDto dto, string shortUrl)
    {
        Alias = dto.Alias;
        Url = dto.Url;
        ExpiresAt = dto.ExpiresAt;
        ShortUrl = shortUrl;
    }
}