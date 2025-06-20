using System;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MatchService.API.Protos;
using MatchService.BusinessLogic.Models.Match;
using MatchService.BusinessLogic.Services.Interfaces;

namespace MatchService.API.Services;

public class TournamentGrpcService : TournamentService.TournamentServiceBase
{
    private readonly IMatchService _matchService;
    public TournamentGrpcService(IMatchService matchService){
        _matchService = matchService;
    }

    public override async Task<Empty> CreateMatches(AddMatchesRequest request, ServerCallContext context)
    {
        List<MatchDto> matchDtos = request.Matches.Select(m => MatchToDto(m)).ToList();
        await _matchService.AddMatchesAsync(matchDtos);
        return new Empty();
    }

    public override async Task<Match> GetMatchById(GetByIdRequest request, ServerCallContext context)
    {
        var matchDto = await _matchService.GetByIdAsync(request.Id);
        return DtoToMatch(matchDto);
    }

    public override async Task<Match> GetMatchByRound(GetByRoundRequest request, ServerCallContext context)
    {
        var matchDto = await _matchService.GetByRoundAsync(request.TournamentId, request.Name);
        return DtoToMatch(matchDto);
    }

    public override async Task<Empty> UpdateMatch(Match request, ServerCallContext context)
    {
        var matchDto = await _matchService.UpdateForGrpcAsync(request.Id, MatchToDto(request), request.OwnerId);
        return new Empty();
    }

    private MatchDto MatchToDto(Match match){
        return new MatchDto(){
            id = match.Id,
            round = match.Round,
            startTime = match.StartTime.ToDateTime(),
            status = (Shared.Enums.MatchStatus)match.Status,
            matchOrder = match.MatchOrder,
            winScore = match.WinScore,
            looseScore = match.LooseScore,
            endTime = match.EndTime.ToDateTime(),
            categoryId = match.CategoryId,
            winnerId = match.WinnerId,
            participant1Id = match.Participant1Id,
            participant2Id = match.Participant2Id,
            tournamentId = match.TournamentId,
            nextMatchId = (match.NextMatchId.Equals(" ") || match.NextMatchId.Equals("")) ? null : match.NextMatchId,
            ownerId = match.OwnerId,
            participant1Name = match.Participant1Name,
            participant2Name = match.Participant2Name,
            tournamentName = match.TournamentName
        };
    }

    private Match DtoToMatch(MatchDto match){
        return new Match(){
            Id = match.id,
            Round = match.round,
            StartTime = match.startTime.ToUniversalTime().ToTimestamp(),
            Status = (int)match.status,
            MatchOrder = match.matchOrder,
            WinScore = match.winScore,
            LooseScore = match.looseScore,
            EndTime = match.endTime.ToUniversalTime().ToTimestamp(),
            CategoryId = match.categoryId,
            WinnerId = match.winnerId,
            Participant1Id = match.participant1Id,
            Participant2Id = match.participant2Id,
            TournamentId = match.tournamentId,
            NextMatchId = string.IsNullOrEmpty(match.nextMatchId) ? " " : match.nextMatchId ,
            OwnerId = match.ownerId,
            Participant1Name = match.participant1Name,
            Participant2Name = match.participant2Name,
            TournamentName = match.tournamentName
        };
    }
}
