using Microsoft.AspNetCore.Identity;

namespace UserService.DataAccess.Entities;

public class User : IdentityUser
{
    public string? Image { get; set; }
    public string? Bio { get; set; }
    public DateTime RegisteredAt { get; set; }
    public List<UserTournaments> userTournaments { get; set; }
}