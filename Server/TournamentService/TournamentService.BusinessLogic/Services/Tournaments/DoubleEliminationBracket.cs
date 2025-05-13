using System;
using System.Runtime.InteropServices;
using TournamentService.BusinessLogic.Models.Match;
using TournamentService.BusinessLogic.Services.Interfaces;
using TournamentService.BusinessLogic.Services.Tournaments.Interfaces;
using TournamentService.DataAccess.Repositories.Interfaces;
using TournamentService.Shared.Constants;
using TournamentService.Shared.Enums;
using TournamentService.Shared.Exceptions;

namespace TournamentService.BusinessLogic.Services.Tournaments;

public class DoubleEliminationBracket : IDoubleEliminationBracket
{
    private readonly IMatchService _matchService;
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IParticipantService _participantService;
    private readonly ISingleEliminationBracket _singleEliminationBracket;

    public DoubleEliminationBracket(IMatchService matchService, ITournamentRepository tournamentRepository, IParticipantService participantService, ISingleEliminationBracket singleEliminationBracket)
    {
        _matchService = matchService;
        _tournamentRepository = tournamentRepository;
        _participantService = participantService;
        _singleEliminationBracket = singleEliminationBracket;
    }

    public async Task GenerateBracket(string tournamentId)
    {
        await _singleEliminationBracket.GenerateBracket(tournamentId);
        var t = _tournamentRepository.GetById(tournamentId);
        _matchService.CreateMatches(new List<MatchDto>()
        {CreateMatch(
            tournamentId,
            "Final",
            1,
            string.Empty,
            string.Empty,
            t.OwnerId,
            t.DisciplineId,
            string.Empty,
            string.Empty,
            t.Name)});
    }

    public async Task GenerateLowerBracket(string tournamentId)
    {
        var res = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (res == null) throw new NotFoundException(ErrorName.TournamentNotFound);
        var participantss = await _participantService.GetAllFromLowerAsync(tournamentId);
        var upper = (await _participantService.GetAllFromUpperAsync(tournamentId)).Count;
        var matches = new List<MatchDto>();

        Random.Shared.Shuffle(CollectionsMarshal.AsSpan(participantss));

        for (int i = 0; i < participantss.Count; i += 2)
        {
            var match = CreateMatch(
                tournamentId,
                $"Lower bracket round with {upper}/{participantss.Count} participants",
                i / 2 + 1,
                participantss[i].Id,
                participantss[i + 1].Id,
                res.OwnerId,
                res.DisciplineId,
                participantss[i].Name,
                participantss[i + 1].Name,
                res.Name);
            matches.Add(match);
        }
        _matchService.CreateMatches(matches);
    }

    public async Task HandleMatchResult(string matchId, string winnerId, string looserId, int winPoints, int loosePoints)
    {
        var match = await _matchService.GetMatchById(matchId);
        if (match == null) throw new NotFoundException(ErrorName.MatchNotFound);
        var winner = await _participantService.GetByIdAsync(winnerId);
        var looser = await _participantService.GetByIdAsync(looserId);

        match.status = MatchStatus.Completed;
        match.winnerId = winnerId;
        match.winScore = winPoints;
        match.looseScore = loosePoints;

        await _matchService.UpdateMatch(match.id, match);

        if (winner.Status == ParticipantStatus.PlayWin)
        {
            await AdvanceInWinners(matchId, winnerId);
            await DropToLosers(looserId, match.ownerId);
        }
        else if (winner.Status == ParticipantStatus.PlayLoose)
        {
            await AdvanceInLosers(winnerId);
            await DropInLoosers(looserId, match.ownerId);
        }
    }

    private async Task AdvanceInWinners(string matchId, string winnerId)
    {
        var match = await _matchService.GetMatchById(matchId);
        if (match == null) throw new NotFoundException(ErrorName.MatchNotFound);
        var winner = await _participantService.GetByIdAsync(winnerId);

        await _participantService.UpdatePointsAsync(winnerId, 1);

        MatchDto nextMatch = new MatchDto();

        if (!string.IsNullOrEmpty(match.nextMatchId))
        {
            nextMatch = await _matchService.GetMatchById(match.nextMatchId);

            if (!match.round.Equals("1/1"))
            {
                if (string.IsNullOrEmpty(nextMatch.participant1Id))
                {
                    nextMatch.participant1Id = winnerId;
                    nextMatch.participant1Name = winner.Name;
                }
                else
                {
                    nextMatch.participant2Id = winnerId;
                    nextMatch.participant2Name = winner.Name;
                }
                await _matchService.UpdateMatch(nextMatch.id, nextMatch);
            }
            else{
                nextMatch = await _matchService.GetMatchByName(match.tournamentId, "Final");
                if (string.IsNullOrEmpty(nextMatch.participant1Id))
                {
                    nextMatch.participant1Id = winnerId;
                    nextMatch.participant1Name = winner.Name;
                }
                else
                {
                    nextMatch.participant2Id = winnerId;
                    nextMatch.participant2Name = winner.Name;
                }
                await _matchService.UpdateMatch(nextMatch.id, nextMatch);
            }
        }
    }

    private async Task DropToLosers(string loserId, string ownerId)
    {
        var participant = await _participantService.GetByIdAsync(loserId);
        participant.Status = ParticipantStatus.PlayLoose;
        await _participantService.UpdateAsync(participant.Id, participant, ownerId);
    }

    private async Task AdvanceInLosers(string winnerId)
    {
        await _participantService.UpdatePointsAsync(winnerId, 1);
    }

    private async Task DropInLoosers(string looserId, string ownerId)
    {
        var looser = await _participantService.GetByIdAsync(looserId);
        looser.Status = ParticipantStatus.Left;
        await _participantService.UpdateAsync(looser.Id, looser, ownerId);
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
