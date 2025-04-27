using System;
using MatchService.Shared.Enums;

namespace MatchService.BusinessLogic.Models.Match;

public class MatchUpdateDto
{
    public DateTime startTime {get; set;}
    public MatchStatus status {get; set;}
    public int matchOrder {get; set;}
    public int winScore {get; set;}
    public int looseScore {get; set;}
    public DateTime endTime {get; set;}
    
    public string categoryId {get; set;}
    public string winnerId {get; set;}
    public string participant1Id {get; set;}
    public string participant2Id {get; set;}
}
