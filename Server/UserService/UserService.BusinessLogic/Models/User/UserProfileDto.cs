using System;

namespace UserService.BusinessLogic.Models.User;

public class UserProfileDto
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Bio { get; set; }
    public DateTime RegisteredAt { get; set; }
    public string Image { get; set; }
    public List<string> tournaments { get; set; }
}
