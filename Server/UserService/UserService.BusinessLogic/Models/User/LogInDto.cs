using System;

namespace UserService.BusinessLogic.Models.User;

public class LogInDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}
