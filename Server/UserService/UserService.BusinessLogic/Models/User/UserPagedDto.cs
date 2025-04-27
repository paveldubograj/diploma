using System;

namespace UserService.BusinessLogic.Models.User;

public class UserPagedDto
{
    public List<UserCleanDto> Users {get; set;}
    public int Total {get; set;}
}
