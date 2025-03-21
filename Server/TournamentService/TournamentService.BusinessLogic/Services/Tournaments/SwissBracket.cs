using System;
using System.Runtime.InteropServices;
using TournamentService.BusinessLogic.Models.Match;
using TournamentService.BusinessLogic.Services.Interfaces;
using TournamentService.DataAccess.Repositories.Interfaces;
using TournamentService.Shared.Constants;
using TournamentService.Shared.Enums;
using TournamentService.Shared.Exceptions;

namespace TournamentService.BusinessLogic.Services.Tournaments;

public class SwissBracket
{
    private readonly IMatchService _matchService;
    private readonly TournamentService _tournamentService;
    private readonly IParticipantService _participantService;

    public SwissBracket(IMatchService matchService, TournamentService tournamentService, IParticipantService participantService)
    {
        _matchService = matchService;
        _tournamentService = tournamentService;
        _participantService = participantService;
    }

    public async Task GenerateSwissMatches(string tournamentId)
    {
        var res = await _tournamentService.GetByIdAsync(tournamentId);
        if(res == null){
            throw new NotFoundException(ErrorName.TournamentNotFound);
        }
        res.Rounds += 1;
        res = await _tournamentService.UpdateAsync(res.Id, res, res.OwnerId);
        var participants = await _participantService.GetAllByTournamentAsync(tournamentId);
        List<MatchDto> matches = new List<MatchDto>();
        if (participants.Count < 2) throw new Exception("Недостаточно участников!");

        participants = participants.OrderBy(p => p.Points).ToList();
        if(res.Rounds == 1) Random.Shared.Shuffle(CollectionsMarshal.AsSpan(participants));
        for (int i = 0; i < participants.Count / 2; i++)
        {
            matches.Add(CreateMatch(
                tournamentId,
                res.Rounds,
                i,
                participants[i * 2].Id,
                participants[i * 2 + 1].Id,
                res.OwnerId,
                res.DisciplineId
            ));
        }
        _matchService.CreateMatches(matches);
    }

    public async Task HandleMatchResult(string matchId, string winnerId, string loserId, int winScore, int looseScore)
    {
        var match = await _matchService.GetMatchById(matchId);
        if (match == null) throw new Exception("Матч не найден!");

        match.Status = MatchStatus.Completed;
        match.WinnerId = winnerId;
        match.WinScore = winScore;
        match.LooseScore = looseScore;

        if(string.IsNullOrEmpty(winnerId)){
            await _participantService.UpdatePointsAsync(winnerId, 1); // 1 очко за ничью
            await _participantService.UpdatePointsAsync(loserId, 1); // 1 очко за ничью
        }
        else{
            match.WinnerId = winnerId;
            await _participantService.UpdatePointsAsync(winnerId, 3); // 3 очка за победу
        }
        await _matchService.UpdateMatch(match.Id, match);        
    }

    private MatchDto CreateMatch(string tournamentId, int round, int number, string participant1Id, string participant2Id, string ownerId, string categoryId){
        MatchDto dto = new MatchDto(){
            Id = new Guid().ToString(),
            TournamentId = tournamentId, 
            Round = round.ToString(), 
            MatchOrder = number, 
            Participant1Id = participant1Id, 
            Participant2Id = participant2Id,
            OwnerId = ownerId,
            CategoryId = categoryId};
        return dto;
    }
}
