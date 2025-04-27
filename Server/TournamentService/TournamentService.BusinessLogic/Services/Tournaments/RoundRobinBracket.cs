using System;
using TournamentService.BusinessLogic.Models.Match;
using TournamentService.BusinessLogic.Services.Interfaces;
using TournamentService.BusinessLogic.Services.Tournaments.Interfaces;
using TournamentService.DataAccess.Repositories.Interfaces;
using TournamentService.Shared.Constants;
using TournamentService.Shared.Enums;
using TournamentService.Shared.Exceptions;

namespace TournamentService.BusinessLogic.Services.Tournaments;

public class RoundRobinBracket : IRoundRobinBracket
{
    private readonly IMatchService _matchService;
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IParticipantService _participantService;

    public RoundRobinBracket(IMatchService matchService, ITournamentRepository tournamentService, IParticipantService participantService)
    {
        _matchService = matchService;
        _tournamentRepository = tournamentService;
        _participantService = participantService;
    }

    public async Task GenerateBracket(string tournamentId)
    {
        var res = await _tournamentRepository.GetByIdAsync(tournamentId);
        if(res == null){
            throw new NotFoundException(ErrorName.TournamentNotFound);
        }
        var participants = await _participantService.GetAllByTournamentAsync(tournamentId);
        if (participants.Count < 2) throw new Exception("Недостаточно участников!");
        int totalPlayers = participants.Count;
        var matches = new List<MatchDto>();

        for (int round = 1; round < totalPlayers; round++)
        {
            for (int i = 0; i < totalPlayers / 2; i++)
            {
                var p1 = participants[i];
                var p2 = participants[totalPlayers - 1 - i];
                matches.Add(CreateMatch(tournamentId, round, i + 1, p1.Id, p2.Id, res.OwnerId, res.DisciplineId));
            }

            // Циклический сдвиг для следующего раунда
            participants.Insert(1, participants.Last());
            participants.RemoveAt(participants.Count - 1);
        }

        _matchService.CreateMatches(matches);
    }
    public async Task HandleMatchResult(string matchId, string winnerId, int winPoints, int loosePoints)
    {
        var match = await _matchService.GetMatchById(matchId);
        if (match == null) throw new NotFoundException(ErrorName.MatchNotFound);
        match.status = MatchStatus.Completed;
        match.winnerId = winnerId;
        match.winScore = winPoints;
        match.looseScore = loosePoints;
        await _participantService.UpdatePointsAsync(winnerId, 1, match.ownerId);
        await _matchService.UpdateMatch(matchId, match);
    }

    private MatchDto CreateMatch(string tournamentId, int round, int number, string participant1Id, string participant2Id, string ownerId, string categoryId){
        MatchDto dto = new MatchDto(){
            //Id = new Guid().ToString(),
            tournamentId = tournamentId, 
            round = round.ToString(), 
            matchOrder = number, 
            participant1Id = participant1Id, 
            participant2Id = participant2Id,
            ownerId = ownerId,
            categoryId = categoryId,
            status = MatchStatus.Scheduled};
        return dto;
    }
}
