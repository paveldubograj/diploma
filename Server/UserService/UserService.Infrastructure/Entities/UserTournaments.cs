using System;

namespace UserService.DataAccess.Entities;

public class UserTournaments
{
    public UserTournaments()
    {
        Id = Guid.NewGuid().ToString();
    }
    public string Id { get; set; }
    public string UserId { get; set; }
    public string TournamentId { get; set; }
    public User User { get; set; }
    public override string ToString()
    {
        return TournamentId;
    }
}
