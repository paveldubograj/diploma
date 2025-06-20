using System;
using MatchService.Shared.Enums;

namespace MatchService.BusinessLogic.Models.Filter;

public class MatchFilter
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public SortOptions? Options { get; set; }
    public string? CategoryId { get; set; }
    public DateTime? StartTime {get; set;}
    public DateTime? EndTime {get; set;}
    public string? TournamentId {get; set;}
    public int? Status {get; set;}
}
