using System;
using TournamentService.Shared.Enums;

namespace TournamentService.BusinessLogic.Models.Filters;

public class TournamentFilter
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public TournamentSortOptions? Options { get; set; }
    public string? SearchString { get; set; }
    public string? CategoryId {get; set;}
    public int? Status {get; set;}
    public int? Format {get; set;}
    public DateTime? StartTime {get; set;}
    public DateTime? EndTime {get; set;} 
}
