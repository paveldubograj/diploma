using System;
using TournamentService.BusinessLogic.Models.Match;
using TournamentService.BusinessLogic.Services.Interfaces;
using TournamentService.BusinessLogic.Services.Tournaments.Interfaces;
using TournamentService.BusinessLogic.Models.ParticipantDtos;
using TournamentService.DataAccess.Repositories.Interfaces;
using TournamentService.Shared.Constants;
using TournamentService.Shared.Enums;
using TournamentService.Shared.Exceptions;

namespace TournamentService.BusinessLogic.Services.Tournaments;

public class RoundRobinBracket : IRoundRobinBracket
{
    private readonly IMatchGrpcService _matchService;
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IParticipantService _participantService;

    public RoundRobinBracket(IMatchGrpcService matchService, ITournamentRepository tournamentService, IParticipantService participantService)
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
        if (participants.Count < 2) throw new ParticipantAmountException(ErrorName.NotEnoughParticipants);

        var matches = new List<MatchDto>();
        int n = participants.Count;

        bool hasBye = n % 2 != 0;
        if (hasBye) n++;

        for (int round = 0; round < n - 1; round++)
        {
            for (int i = 0; i < n / 2; i++)
            {
                int p1Index = i;
                int p2Index = n - 1 - i;

                if (hasBye && (p1Index >= participants.Count || p2Index >= participants.Count))
                    continue;

                var p1 = participants[p1Index];
                var p2 = participants[p2Index];

                matches.Add(CreateMatch(
                    tournamentId,
                    $"{round + 1}",
                    i + 1,
                    p1.Id, p2.Id,
                    res.OwnerId,
                    res.DisciplineId,
                    p1.Name, p2.Name,
                    res.Name));
            }
            var rotated = new List<ParticipantDto>(participants);
            var temp = rotated[1];
            rotated.RemoveAt(1);
            rotated.Add(temp);
            participants = rotated;
        }

        await _matchService.CreateMatches(matches);
    }
    public async Task HandleMatchResult(string matchId, string winnerId, int winPoints, int loosePoints)
    {
        var match = await _matchService.GetMatchById(matchId);
        if (match == null) throw new NotFoundException(ErrorName.MatchNotFound);
        match.status = MatchStatus.Completed;
        match.winnerId = winnerId;
        match.winScore = winPoints;
        match.looseScore = loosePoints;
        await _participantService.UpdatePointsAsync(winnerId, 1);
        await _matchService.UpdateMatch(matchId, match);
    }

    private MatchDto CreateMatch(string tournamentId, 
        string round, 
        int number, 
        string participant1Id, 
        string participant2Id, 
        string ownerId, 
        string categoryId, 
        string participant1Name, 
        string participant2Name, 
        string tournamentName)
    {
        MatchDto dto = new MatchDto()
        {
            id = Guid.NewGuid().ToString(),
            tournamentId = tournamentId,
            round = round,
            matchOrder = number,
            participant1Id = participant1Id,
            participant2Id = participant2Id,
            winnerId = string.Empty,
            winScore = 0,
            ownerId = ownerId,
            categoryId = categoryId,
            nextMatchId = string.Empty,
            participant1Name = participant1Name,
            participant2Name = participant2Name,
            tournamentName = tournamentName
        };
        return dto;
    }
}
