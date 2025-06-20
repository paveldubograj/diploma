using System;

namespace UserService.BusinessLogic.Models.User;

public class UserUpdateDto
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Bio { get; set; }
}
