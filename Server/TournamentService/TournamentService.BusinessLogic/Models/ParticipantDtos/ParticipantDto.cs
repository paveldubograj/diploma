using System;
using TournamentService.Shared.Enums;

namespace TournamentService.BusinessLogic.Models.ParticipantDtos;

public class ParticipantDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string UserId {get; set;}
    public int Points {get; set;}
    public string TournamentId {get; set;}
    public ParticipantStatus Status {get; set;}
}
