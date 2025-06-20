using System;

namespace MatchService.DataAccess.Entities;

public class MatchList
{
    public List<Match> Matches { get; set; }
    public int Total { get; set; }
}
