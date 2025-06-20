using System;
using System.Runtime.InteropServices;
using TournamentService.BusinessLogic.Models.Match;
using TournamentService.BusinessLogic.Services.Interfaces;
using TournamentService.BusinessLogic.Services.Tournaments.Interfaces;
using TournamentService.DataAccess.Entities;
using TournamentService.DataAccess.Repositories.Interfaces;
using TournamentService.Shared.Constants;
using TournamentService.Shared.Enums;
using TournamentService.Shared.Exceptions;

namespace TournamentService.BusinessLogic.Services.Tournaments;

public class SingleEliminationBracket : ISingleEliminationBracket
{
    private readonly IMatchGrpcService _matchService;
    private readonly ITournamentRepository _tournamentRepository;
    //private readonly IParticipantService _participantService;
    private readonly IParticipantRepository _participantService;

    public SingleEliminationBracket(IMatchGrpcService matchService, ITournamentRepository tournamentRepository, IParticipantRepository participantRepository)
    {
        _matchService = matchService;
        _tournamentRepository = tournamentRepository;
        _participantService = participantRepository;
    }

    public async Task GenerateBracket(string tournamentId)
    {
        var res = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (res == null)
        {
            throw new NotFoundException(ErrorName.TournamentNotFound);
        }
        var participantss = await _participantService.GetAllAsync(tournamentId);
        int totalPlayers = participantss.Count;
        int powerOfTwo = res.MaxParticipants;

        while (participantss.Count < powerOfTwo)
            participantss.Add(new Participant(){Id = "PASS", Name = "PASS"}); 

        var matches = new List<MatchDto>();
        var previousRoundMatches = new List<MatchDto>();

        Random.Shared.Shuffle(CollectionsMarshal.AsSpan(participantss));

        for (int i = 0; i < participantss.Count; i += 2)
        {
            var match = CreateMatch(
                tournamentId, 
                $"1/{totalPlayers / 2}", 
                i / 2 + 1, 
                participantss[i].Id, 
                participantss[i + 1].Id, 
                res.OwnerId, 
                res.DisciplineId, 
                participantss[i].Name, 
                participantss[i + 1].Name, 
                res.Name);
            previousRoundMatches.Add(match);
        }

        int round = 1;
        if (previousRoundMatches.Count > 1) round = 2;
        while (previousRoundMatches.Count > 1)
        {
            var currentRoundMatches = new List<MatchDto>();
            for (int i = 0; i < previousRoundMatches.Count; i += 2)
            {
                var match = CreateMatch(
                    tournamentId, 
                    $"1/{totalPlayers / Math.Pow(2, round)}", 
                    i / 2 + 1, 
                    string.Empty, 
                    string.Empty, 
                    res.OwnerId, 
                    res.DisciplineId,
                    string.Empty,
                    string.Empty,
                    res.Name);
                previousRoundMatches[i].nextMatchId = match.id;
                previousRoundMatches[i + 1].nextMatchId = match.id;
                currentRoundMatches.Add(match);
            }
            matches.AddRange(previousRoundMatches);
            previousRoundMatches = currentRoundMatches;
            round++;
        }
        matches.AddRange(previousRoundMatches);
        if (matches.Count > 1) 
            matches.Add(
                CreateMatch(
                    tournamentId, 
                    "3-rd place match", 
                    1, 
                    string.Empty, 
                    string.Empty, 
                    res.OwnerId, 
                    res.DisciplineId,
                    string.Empty,
                    string.Empty,
                    res.Name));
        res.Rounds = round;
        await _tournamentRepository.UpdateAsync(res);

        await _matchService.CreateMatches(matches);
    }

    public async Task<bool> HandleMatchResult(string matchId, string winnerId, string looserId, int winPoints, int loosePoints)
    {
        var match = await _matchService.GetMatchById(matchId);
        if (match == null) throw new NotFoundException(ErrorName.MatchNotFound);
        var winner = await _participantService.GetByIdAsync(winnerId);
        var looser = await _participantService.GetByIdAsync(looserId);
        await _participantService.UpdatePointsAsync(winnerId, 1);

        match.status = MatchStatus.Completed;
        match.winnerId = winnerId;
        match.winScore = winPoints;
        match.looseScore = loosePoints;
        await _matchService.UpdateMatch(match.id, match);

        MatchDto nextMatch = new MatchDto();
        if (!string.IsNullOrEmpty(match.nextMatchId))
        {
            nextMatch = await _matchService.GetMatchById(match.nextMatchId);
            if (match.round.Equals("1/2"))
            {
                var third = await _matchService.GetMatchByName(match.tournamentId, "3-rd place match");
                if (string.IsNullOrEmpty(third.participant1Id)) {
                    third.participant1Id = looserId;
                    third.participant1Name = looser.Name;
                }
                else {
                    third.participant2Id = looserId;
                    third.participant2Name = looser.Name;
                }
                await _matchService.UpdateMatch(third.id, third);
            }
            if (!match.round.Equals("1/1"))
            {
                if (string.IsNullOrEmpty(nextMatch.participant1Id)) {
                    nextMatch.participant1Id = winnerId;
                    nextMatch.participant1Name = winner.Name;
                }
                else {
                    nextMatch.participant2Id = winnerId;
                    nextMatch.participant2Name = winner.Name;
                }
                await _matchService.UpdateMatch(nextMatch.id, nextMatch);
            }
        }
        return true;
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