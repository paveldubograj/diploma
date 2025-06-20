using System;
using TournamentService.BusinessLogic.Models.ParticipantDtos;
using TournamentService.DataAccess.Entities;
using TournamentService.Shared.Enums;

namespace TournamentService.BusinessLogic.Models.Tournament;

public class TournamentDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string DisciplineId { get; set; }
    public TournamentStatus Status {get; set;}
    public TournamentFormat Format { get; set; } 
    public int Rounds {get; set;}
    public int MaxParticipants { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<ParticipantDto> Participants { get; set; } = new List<ParticipantDto>();
    public bool IsRegistrationAllowed {get; set;}
    public string ImagePath {get; set;}
    
    public string OwnerId {get; set;}
    public string WinnerId {get; set;}
}
