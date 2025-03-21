using System;
using System.Runtime.InteropServices;
using TournamentService.BusinessLogic.Models.Match;
using TournamentService.BusinessLogic.Services.Interfaces;
using TournamentService.DataAccess.Repositories.Interfaces;
using TournamentService.Shared.Constants;
using TournamentService.Shared.Enums;
using TournamentService.Shared.Exceptions;

namespace TournamentService.BusinessLogic.Services.Tournaments;

public class SingleEliminationBracket
{
    private readonly IMatchService _matchService;
    private readonly ITournamentService _tournamentService;
    private readonly IParticipantService _participantService;

    public SingleEliminationBracket(IMatchService matchService, ITournamentService tournamentService, IParticipantService participantService)
    {
        _matchService = matchService;
        _tournamentService = tournamentService;
        _participantService = participantService;
    }

    public async Task GenerateBracket(string tournamentId, List<string> participants)
    {
        var res = await _tournamentService.GetByIdAsync(tournamentId);
        if(res == null){
            throw new NotFoundException(ErrorName.TournamentNotFound);
        }
        var participantss = await _participantService.GetAllByTournamentAsync(tournamentId);
        int totalPlayers = participantss.Count;
        int powerOfTwo = (int)Math.Pow(2, Math.Ceiling(Math.Log2(totalPlayers)));

        // Добавляем "BYE" при необходимости
        while (participantss.Count < powerOfTwo)
            participantss.Add(null); 

        var matches = new List<MatchDto>();
        var previousRoundMatches = new List<MatchDto>();

        Random.Shared.Shuffle(CollectionsMarshal.AsSpan(participantss));
        
        // Первый раунд
        for (int i = 0; i < participantss.Count; i += 2)
        {
            var match = CreateMatch(tournamentId, $"1/{totalPlayers}", i / 2 + 1, participantss[i].Id, participantss[i + 1].Id, res.OwnerId, res.DisciplineId);
            matches.Add(match);
            previousRoundMatches.Add(match);
        }

        // Заполняем следующими раундами
        int round = 2;
        while (previousRoundMatches.Count > 1)
        {
            var currentRoundMatches = new List<MatchDto>();
            for (int i = 0; i < previousRoundMatches.Count; i += 2)
            {
                var match = CreateMatch(tournamentId, $"1/{totalPlayers/Math.Pow(2,round-1)}", i / 2 + 1, string.Empty, string.Empty, res.OwnerId, res.DisciplineId);
                matches.FirstOrDefault(c => c.Id.Equals(previousRoundMatches[i].Id)).NextMatchId = match.Id;
                matches.FirstOrDefault(c => c.Id.Equals(previousRoundMatches[i+1].Id)).NextMatchId = match.Id;
                //previousRoundMatches[i].NextMatchId = match.Id;
                //previousRoundMatches[i + 1].NextMatchId = match.Id;
                currentRoundMatches.Add(match);
            }
            matches.AddRange(currentRoundMatches);
            previousRoundMatches = currentRoundMatches;
            round++;
        }
        matches.Add(CreateMatch(tournamentId, "3-rd place match", 1, string.Empty, string.Empty, res.OwnerId, res.DisciplineId));

        _matchService.CreateMatches(matches);
    }

    public async Task HandleMatchResult(string matchId, string winnerId, string looserId)
    {
        var match = await _matchService.GetMatchById(matchId);
        if (match == null) throw new NotFoundException(ErrorName.MatchNotFound);

        match.Status = MatchStatus.Completed;
        match.WinnerId = winnerId;
        await _matchService.UpdateMatch(match.Id, match);

        var nextMatch = await _matchService.GetMatchById(match.NextMatchId);

        if (!match.Round.Equals("1/1"))
        {
            if(string.IsNullOrEmpty(nextMatch.Participant1Id)) nextMatch.Participant1Id = winnerId;
            else nextMatch.Participant2Id = winnerId;
            await _matchService.UpdateMatch(nextMatch.Id, nextMatch);
        }
        else await _tournamentService.EndTournamentAsync(match.TournamentId);

        if(match.Round.Equals("1/2")){
            var third = await _matchService.GetMatchByName("3-rd place match");
            if(string.IsNullOrEmpty(third.Participant1Id)) third.Participant1Id = looserId;
            else third.Participant2Id = looserId;
            await _matchService.UpdateMatch(third.Id, third);
        }
    }

    private MatchDto CreateMatch(string tournamentId, string round, int number, string participant1Id, string participant2Id, string ownerId, string categoryId){
        MatchDto dto = new MatchDto(){
            Id = new Guid().ToString(),
            TournamentId = tournamentId, 
            Round = round, 
            MatchOrder = number, 
            Participant1Id = participant1Id, 
            Participant2Id = participant2Id,
            OwnerId = ownerId,
            CategoryId = categoryId};
        return dto;
    }
}