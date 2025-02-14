using Microsoft.AspNetCore.Identity;

namespace UserService.DataAccess.Entities;

public class User : IdentityUser
{
    public string UserName {get; set;}
    public string Image {get; set;}
}