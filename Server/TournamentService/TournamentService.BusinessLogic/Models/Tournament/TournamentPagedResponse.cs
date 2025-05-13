using System;

namespace TournamentService.BusinessLogic.Models.Tournament;

public class TournamentPagedResponse
{
    public List<TournamentCleanDto> Tournaments {get; set;} = [];
    public int Total {get; set;}
}
