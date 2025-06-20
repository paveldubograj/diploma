using System;
using MatchService.Shared.Enums;

namespace MatchService.BusinessLogic.Models.Filter;

public class MatchFilter
{
    public string? CategoryId {get; set;}
    public DateTime? StartTime {get; set;}
    public DateTime? EndTime {get; set;}
    public string? TournamentId {get; set;}
    public int? Status {get; set;}
}
