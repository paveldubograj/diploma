using System;
using System.Runtime.CompilerServices;
using MatchService.Shared.Enums;

namespace MatchService.DataAccess.Entities;

public class Match
{
    public Match(){
        Id = Guid.NewGuid().ToString();
    }
    public string Id {get; set;}
    public string Round {get; set;}
    public DateTime? StartTime {get; set;}
    public MatchStatus Status {get; set;}
    public int MatchOrder {get; set;}
    public int? WinScore {get; set;}
    public int? LooseScore {get; set;}
    public DateTime? EndTime {get; set;}
    public string Participant1Name {get; set;}
    public string Participant2Name {get; set;}
    public string TournamentName {get; set;}
    
    public string CategoryId {get; set;}
    public string? WinnerId {get; set;}
    public string Participant1Id {get; set;}
    public string Participant2Id {get; set;}
    public string TournamentId {get; set;}
    public string? NextMatchId {get; set;}
    public string OwnerId {get; set;}
    public Match? NextMatch {get; set;}
}
