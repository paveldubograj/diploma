using System;
using System.Text;
using System.Text.Json;
using Grpc.Net.Client;
using TournamentService.BusinessLogic.Protos;
using TournamentService.BusinessLogic.Models.Match;
using TournamentService.BusinessLogic.Services.Interfaces;
using Google.Protobuf.WellKnownTypes;
using TournamentService.Shared.Constants;
using TournamentService.Shared.Exceptions;
using Microsoft.Extensions.Options;
using TournamentService.Shared.Options;

namespace TournamentService.BusinessLogic.Services;

public class MatchGrpcService : IMatchGrpcService
{
    public MatchGrpcService(IOptions<GrpcMatchSettings> options)
    {
        client = new Protos.TournamentService.TournamentServiceClient(GrpcChannel.ForAddress(options.Value.Address, new GrpcChannelOptions
        {
            HttpHandler = new HttpClientHandler()
        }));
    }
    private readonly Protos.TournamentService.TournamentServiceClient client;

    public async Task CreateMatches(List<MatchDto> matches)
    {
        AddMatchesRequest req = new AddMatchesRequest();
        req.Matches.AddRange(matches.Select(m => DtoToMatch(m)));
        try
        {
            await client.CreateMatchesAsync(req);
        }
        catch (Exception ex)
        {
            throw new GrpcException(ErrorName.MatchServiceNotWork);
        }
    }

    public async Task<MatchDto> GetMatchById(string matchId)
    {
        if(string.IsNullOrEmpty(matchId)) throw new NotFoundException(ErrorName.ProvidedIdIsNull);
        Match match;
        try
        {
            match = await client.GetMatchByIdAsync(new GetByIdRequest() { Id = matchId });
        }
        catch (Exception ex)
        {
            throw new GrpcException(ErrorName.MatchServiceNotWork);
        }
        return MatchToDto(match);
    }

    public async Task<MatchDto> GetMatchByName(string tournamentId, string name)
    {
        if(string.IsNullOrEmpty(tournamentId)) throw new NotFoundException(ErrorName.ProvidedIdIsNull);
        if(string.IsNullOrEmpty(name)) throw new NotFoundException(ErrorName.ProvidedNameIsNull);
        Match match;
        try
        {
            match = await client.GetMatchByRoundAsync(new GetByRoundRequest()
            {
                Name = name,
                TournamentId = tournamentId
            });
        }
        catch (Exception ex)
        {
            throw new GrpcException(ErrorName.MatchServiceNotWork);
        }
        return MatchToDto(match);
    }

    public async Task UpdateMatch(string matchId, MatchDto match)
    {
        if(string.IsNullOrEmpty(matchId)) throw new NotFoundException(ErrorName.ProvidedIdIsNull);
        if(match is null) throw new NotFoundException(ErrorName.ProvidedMatchIsNull);
        try
        {
            await client.UpdateMatchAsync(DtoToMatch(match));
        }
        catch (Exception ex)
        {
            throw new GrpcException(ErrorName.MatchServiceNotWork);
        }
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
            nextMatchId = match.NextMatchId,
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
            NextMatchId = match.nextMatchId,
            OwnerId = match.ownerId,
            Participant1Name = match.participant1Name,
            Participant2Name = match.participant2Name,
            TournamentName = match.tournamentName
        };
    }
}
