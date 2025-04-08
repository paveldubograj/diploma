using System;
using TournamentService.Shared.Enums;

namespace TournamentService.DataAccess.Entities;

public class Participant
{
    public Participant(){
        Id = new Guid().ToString();
    }
    public string Id { get; set; }
    public string Name { get; set; }
    public int Points {get; set;}
    public ParticipantStatus Status {get; set;}

    public string TournamentId {get; set;}
    public Tournament Tournament {get; set;}
}
