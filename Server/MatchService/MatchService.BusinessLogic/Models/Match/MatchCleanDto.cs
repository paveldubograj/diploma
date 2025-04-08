using System;
using MatchService.Shared.Enums;

namespace MatchService.BusinessLogic.Models.Match;

public class MatchCleanDto
{
    public string Id {get; set;}
    public int Round { get; set; }
    public DateTime StartTime {get; set;}
    public MatchStatus Status {get; set;}
    public int MatchOrder {get; set;}
    public int WinScore {get; set;}
    public int LooseScore {get; set;}
    public DateTime EndTime {get; set;}
}
