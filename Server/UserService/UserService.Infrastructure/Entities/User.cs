using Microsoft.AspNetCore.Identity;

namespace UserService.DataAccess.Entities;

public class User : IdentityUser
{
    public string Image {get; set;}
}