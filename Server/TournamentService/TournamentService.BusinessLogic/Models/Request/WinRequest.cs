using System;

namespace TournamentService.BusinessLogic.Models.Request;

public class WinRequest
{
    public string WinnerId {get; set;}
    public string LooserId {get; set;}
    public int WinPoints {get; set;}
    public int LoosePoints {get; set;}
}
