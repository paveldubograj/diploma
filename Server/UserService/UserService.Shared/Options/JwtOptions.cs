using System;

namespace UserService.Shared.Options;

public class JwtOptions
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string Key { get; set; }
    public int Expires { get; set; }
}
