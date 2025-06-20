using System;
using TournamentService.Shared.Enums;

namespace TournamentService.DataAccess.Entities;

public class Tournament
{
    public Tournament(){
        Id = new Guid().ToString();
    }
    public string Id { get; set; }
    public string Name { get; set; }
    public TournamentStatus Status {get; set;}
    public TournamentFormat Format { get; set; } 
    public int Rounds {get; set;}
    public int MaxParticipants { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsRegistrationAllowed {get; set;} = false;
    public string ImagePath { get; set; } = string.Empty;
    public List<Participant> Participants { get; set; } = new List<Participant>();

    public string DisciplineId { get; set; }
    public string? WinnerId {get; set;}
    public string OwnerId {get; set;}
}
