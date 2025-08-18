using UrlAlias.Backend.DTos;

namespace UrlAlias.Backend.Dtos.Responses;

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

public class GetAliasesResponse {
    public required List<AliasCreatedResponse> Aliases { get; init; }
    public required int TotalAliases { get; init; }
    public required int TotalPages { get; init; }
}