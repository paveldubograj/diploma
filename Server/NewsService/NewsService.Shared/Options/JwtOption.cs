using System;

namespace NewsService.Shared.Options;

public class JwtOption
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string Key { get; set; }
}
