using System;

namespace MatchService.BusinessLogic.Models.Match;

public class MatchPagedResponse
{
    public List<MatchListDto> Matches {get; set;}
    public int Total {get; set;}
}
