using System;
using TournamentService.Shared.Enums;

namespace TournamentService.BusinessLogic.Models.Match;

public class MatchDto
{
    public string id {get; set;}

    public string round { get; set; }
    public DateTime startTime {get; set;}
    public MatchStatus status {get; set;}
    public int matchOrder {get; set;}
    public int winScore {get; set;}
    public int looseScore {get; set;}
    public DateTime endTime {get; set;}
    public string participant1Name {get; set;}
    public string participant2Name {get; set;}
    public string tournamentName {get; set;}
    
    public string categoryId {get; set;}
    public string winnerId {get; set;}
    public string participant1Id {get; set;}
    public string participant2Id {get; set;}
    public string tournamentId { get; set; }
    public string? nextMatchId {get; set;}
    public string ownerId {get; set;}
}
