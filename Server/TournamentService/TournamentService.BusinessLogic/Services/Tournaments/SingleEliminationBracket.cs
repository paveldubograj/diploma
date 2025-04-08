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
    private readonly IMatchService _matchService;
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IParticipantRepository _participantRepository;

    public SingleEliminationBracket(IMatchService matchService, ITournamentRepository tournamentRepository, IParticipantRepository participantRepository)
    {
        _matchService = matchService;
        _tournamentRepository = tournamentRepository;
        _participantRepository = participantRepository;
    }

    public async Task GenerateBracket(string tournamentId)
    {
        var res = await _tournamentRepository.GetByIdAsync(tournamentId);
        Console.WriteLine(res.Id);
        if(res == null){
            throw new NotFoundException(ErrorName.TournamentNotFound);
        }
        var participantss = _participantRepository.GetAllAsync(tournamentId);
        int totalPlayers = participantss.Count;
        int powerOfTwo = (int)Math.Pow(2, Math.Ceiling(Math.Log2(totalPlayers)));

        // Добавляем "BYE" при необходимости
        while (participantss.Count < powerOfTwo)
            participantss.Add(new Participant(){Id = "PASS", Name = "PASS"}); 

        var matches = new List<MatchDto>();
        var previousRoundMatches = new List<MatchDto>();

        Random.Shared.Shuffle(CollectionsMarshal.AsSpan(participantss));
        
        // Первый раунд
        for (int i = 0; i < participantss.Count; i += 2)
        {
            var match = CreateMatch(tournamentId, $"1/{totalPlayers/2}", i / 2 + 1, participantss[i].Id, participantss[i + 1].Id, res.OwnerId, res.DisciplineId);
            matches.Add(match);
            previousRoundMatches.Add(match);
        }

        // Заполняем следующими раундами
        int round = 1;
        if(previousRoundMatches.Count > 1 ) round = 2;
        while (previousRoundMatches.Count > 1)
        {
            var currentRoundMatches = new List<MatchDto>();
            for (int i = 0; i < previousRoundMatches.Count; i += 2)
            {
                var match = CreateMatch(tournamentId, $"1/{totalPlayers/Math.Pow(2,round-1)}", i / 2 + 1, string.Empty, string.Empty, res.OwnerId, res.DisciplineId);
                matches.FirstOrDefault(c => c.id.Equals(previousRoundMatches[i].id)).nextMatchId = match.id;
                matches.FirstOrDefault(c => c.id.Equals(previousRoundMatches[i+1].id)).nextMatchId = match.id;
                //previousRoundMatches[i].NextMatchId = match.Id;
                //previousRoundMatches[i + 1].NextMatchId = match.Id;
                currentRoundMatches.Add(match);
            }
            matches.AddRange(currentRoundMatches);
            previousRoundMatches = currentRoundMatches;
            round++;
        }
        if(matches.Count > 1) matches.Add(CreateMatch(tournamentId, "3-rd place match", 1, string.Empty, string.Empty, res.OwnerId, res.DisciplineId));
        res.Rounds = round;
        await _tournamentRepository.UpdateAsync(res);

        _matchService.CreateMatches(matches);
    }

    public async Task HandleMatchResult(string matchId, string winnerId, string looserId, int winPoints, int loosePoints)
    {
        var match = await _matchService.GetMatchById(matchId);
        if (match == null) throw new NotFoundException(ErrorName.MatchNotFound);

        match.status = MatchStatus.Completed;
        match.winnerId = winnerId;
        match.winScore = winPoints;
        match.looseScore = loosePoints;
        await _matchService.UpdateMatch(match.id, match);

        MatchDto nextMatch = new MatchDto();

        if (match.nextMatchId != null) nextMatch = await _matchService.GetMatchById(match.nextMatchId);

        if (!match.round.Equals("1/1"))
        {
            if(string.IsNullOrEmpty(nextMatch.participant1Id)) nextMatch.participant1Id = winnerId;
            else nextMatch.participant2Id = winnerId;
            await _matchService.UpdateMatch(nextMatch.id, nextMatch);
        }
        //else await _tournamentRepository.EndTournamentAsync(match.TournamentId);
        else if(match.round.Equals("1/2")){
            var third = await _matchService.GetMatchByName(match.tournamentId, "3-rd place match");
            if(string.IsNullOrEmpty(third.participant1Id)) third.participant1Id = looserId;
            else third.participant2Id = looserId;
            await _matchService.UpdateMatch(third.id, third);
        }
    }

    private MatchDto CreateMatch(string tournamentId, string round, int number, string participant1Id, string participant2Id, string ownerId, string categoryId){
        MatchDto dto = new MatchDto(){
            id = Guid.NewGuid().ToString(),
            tournamentId = tournamentId, 
            round = round, 
            matchOrder = number, 
            participant1Id = participant1Id, 
            participant2Id = participant2Id,
            ownerId = ownerId,
            categoryId = categoryId};
        return dto;
    }
}