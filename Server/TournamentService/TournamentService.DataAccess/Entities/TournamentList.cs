using System;

namespace TournamentService.DataAccess.Entities;

public class TournamentList
{
    public List<Tournament> Tournaments { get; set; }
    public int Total { get; set; }
}
