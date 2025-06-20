using System;

namespace UserService.DataAccess.Entities;

public class UserList
{
    public List<User> Users { get; set; }
    public int Total { get; set; }
}
