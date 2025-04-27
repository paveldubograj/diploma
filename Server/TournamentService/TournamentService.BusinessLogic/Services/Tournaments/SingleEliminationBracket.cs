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
    private readonly IParticipantService _participantService;
    //private readonly IParticipantRepository _participantService;

    public SingleEliminationBracket(IMatchService matchService, ITournamentRepository tournamentRepository, IParticipantService participantRepository)
    {
        _matchService = matchService;
        _tournamentRepository = tournamentRepository;
        _participantService = participantRepository;
    }

    public async Task GenerateBracket(string tournamentId)
    {
        var res = await _tournamentRepository.GetByIdAsync(tournamentId);
        Console.WriteLine(res.Id);
        if (res == null)
        {
            throw new NotFoundException(ErrorName.TournamentNotFound);
        }
        var participantss = await _participantService.GetAllByTournamentAsync(tournamentId);
        //var participantss = _participantService.GetAllAsync(tournamentId);
        int totalPlayers = participantss.Count;
        int powerOfTwo = (int)Math.Pow(2, Math.Ceiling(Math.Log2(totalPlayers)));

        // Добавляем "BYE" при необходимости
        while (participantss.Count < powerOfTwo)
            participantss.Add(new Models.ParticipantDtos.ParticipantDto() { Id = "PASS", Name = "PASS" });
        //participantss.Add(new Participant(){Id = "PASS", Name = "PASS"}); 

        var matches = new List<MatchDto>();
        var previousRoundMatches = new List<MatchDto>();

        Random.Shared.Shuffle(CollectionsMarshal.AsSpan(participantss));

        // Первый раунд
        for (int i = 0; i < participantss.Count; i += 2)
        {
            var match = CreateMatch(tournamentId, $"1/{totalPlayers / 2}", i / 2 + 1, participantss[i].Id, participantss[i + 1].Id, res.OwnerId, res.DisciplineId);
            //matches.Add(match);
            previousRoundMatches.Add(match);
        }

        // Заполняем следующими раундами
        int round = 1;
        if (previousRoundMatches.Count > 1) round = 2;
        while (previousRoundMatches.Count > 1)
        {
            var currentRoundMatches = new List<MatchDto>();
            for (int i = 0; i < previousRoundMatches.Count; i += 2)
            {
                var match = CreateMatch(tournamentId, $"1/{totalPlayers / Math.Pow(2, round)}", i / 2 + 1, string.Empty, string.Empty, res.OwnerId, res.DisciplineId);
                previousRoundMatches[i].nextMatchId = match.id;
                previousRoundMatches[i + 1].nextMatchId = match.id;
                currentRoundMatches.Add(match);
            }
            matches.AddRange(previousRoundMatches);
            previousRoundMatches = currentRoundMatches;
            round++;
        }
        matches.AddRange(previousRoundMatches);
        if (matches.Count > 1) matches.Add(CreateMatch(tournamentId, "3-rd place match", 1, string.Empty, string.Empty, res.OwnerId, res.DisciplineId));
        res.Rounds = round;
        await _tournamentRepository.UpdateAsync(res);

        _matchService.CreateMatches(matches);
    }

    public async Task<bool> HandleMatchResult(string matchId, string winnerId, string looserId, int winPoints, int loosePoints)
    {
        var match = await _matchService.GetMatchById(matchId);
        if (match == null) throw new NotFoundException(ErrorName.MatchNotFound);

        Console.WriteLine("Получил");

        // Console.WriteLine("Получаю винера");
        // var winner = _tournamentRepository.GetParticipantById(winnerId);
        // winner.Points += 1;
        // Console.WriteLine("Обновляю винера");
        // _participantService.Update(winner);
        // Console.WriteLine("Обновил винера");


        // Console.WriteLine("Обновляю винера");

        // await _participantService.UpdatePointsAsync(winnerId, 1, match.ownerId);

        // Console.WriteLine("Обновил винера");

        match.status = MatchStatus.Completed;
        match.winnerId = winnerId;
        match.winScore = winPoints;
        match.looseScore = loosePoints;

        Console.WriteLine("Параметры сменил");

        await _matchService.UpdateMatch(match.id, match);

        Console.WriteLine("Апдейтнул");

        MatchDto nextMatch = new MatchDto();

        if (!string.IsNullOrEmpty(match.nextMatchId))
        {
            nextMatch = await _matchService.GetMatchById(match.nextMatchId);

            Console.WriteLine("Получил следующий");

            if (match.round.Equals("1/2"))
            {
                var third = await _matchService.GetMatchByName(match.tournamentId, "3-rd place match");
                if (string.IsNullOrEmpty(third.participant1Id)) third.participant1Id = looserId;
                else third.participant2Id = looserId;
                await _matchService.UpdateMatch(third.id, third);
            }
            if (!match.round.Equals("1/1"))
            {
                if (string.IsNullOrEmpty(nextMatch.participant1Id)) nextMatch.participant1Id = winnerId;
                else nextMatch.participant2Id = winnerId;
                await _matchService.UpdateMatch(nextMatch.id, nextMatch);
            }

            Console.WriteLine("Обновил следующий");
        }



        return true;


        //else await _tournamentRepository.EndTournamentAsync(match.TournamentId);
        // else if(match.round.Equals("1/2")){
        //     var third = await _matchService.GetMatchByName(match.tournamentId, "3-rd place match");
        //     if(string.IsNullOrEmpty(third.participant1Id)) third.participant1Id = looserId;
        //     else third.participant2Id = looserId;
        //     await _matchService.UpdateMatch(third.id, third);
        // }
    }

    private MatchDto CreateMatch(string tournamentId, string round, int number, string participant1Id, string participant2Id, string ownerId, string categoryId)
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
            nextMatchId = string.Empty
        };
        return dto;
    }
}