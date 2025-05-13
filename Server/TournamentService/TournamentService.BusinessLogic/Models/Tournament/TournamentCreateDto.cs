using System;
using TournamentService.Shared.Enums;

namespace TournamentService.BusinessLogic.Models.Tournament;

public class TournamentCreateDto
{
    public string Name { get; set; }
    public string DisciplineId { get; set; }
    public TournamentFormat Format { get; set; } 
    public int MaxParticipants { get; set; }
    public bool IsRegistrationAllowed {get; set;}
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
