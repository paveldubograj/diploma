using System;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using TournamentService.BusinessLogic.Protos;
using TournamentService.BusinessLogic.Services.Interfaces;
using TournamentService.Shared.Options;

namespace TournamentService.BusinessLogic.Services;

public class UserGrpcService : IUserGrpcService
{
    public UserGrpcService(IOptions<GrpcUserSettings> options)
    {
        client = new Protos.TournamentRegisterService.TournamentRegisterServiceClient(GrpcChannel.ForAddress(options.Value.Address, new GrpcChannelOptions
        {
            HttpHandler = new HttpClientHandler()
        }));
    }
    private readonly Protos.TournamentRegisterService.TournamentRegisterServiceClient client;
    public async Task AddToUser(string userId, string tournamentId)
    {
        Request r = new Request() { UserId = userId, TournamentId = tournamentId };
        await client.AddToUserAsync(r);
    }

    public async Task RemoveFromUser(string userId, string tournamentId)
    {
        Request r = new Request() { UserId = userId, TournamentId = tournamentId };
        await client.RemoveFromUserAsync(r);
    }
}
