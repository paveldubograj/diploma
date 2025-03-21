using System;
using TournamentService.Shared.Enums;

namespace TournamentService.BusinessLogic.Models.Match;

public class MatchDto
{
    public string Id {get; set;}

    public string Round { get; set; }
    public DateTime StartTime {get; set;}
    public MatchStatus Status {get; set;}
    public int MatchOrder {get; set;}
    public int WinScore {get; set;}
    public int LooseScore {get; set;}
    public DateTime EndTime {get; set;}
    
    public string CategoryId {get; set;}
    public string WinnerId {get; set;}
    public string Participant1Id {get; set;}
    public string Participant2Id {get; set;}
    public string TournamentId { get; set; }
    public string? NextMatchId {get; set;}
    public string OwnerId {get; set;}
}
