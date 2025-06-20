using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using TournamentService.BusinessLogic.Protos;
using TournamentService.BusinessLogic.Services.Interfaces;
using TournamentService.Shared.Constants;
using TournamentService.Shared.Exceptions;
using TournamentService.Shared.Options;

namespace TournamentService.BusinessLogic.Services.Tournaments;

public class DisciplineGrpcService : IDisciplineGrpcService
{
    public DisciplineGrpcService(IOptions<GrpcDisciplineSettings> options)
    {
        client = new Protos.DisciplineService.DisciplineServiceClient(GrpcChannel.ForAddress(options.Value.Address, new GrpcChannelOptions
        {
            HttpHandler = new HttpClientHandler()
        }));
    }
    private readonly Protos.DisciplineService.DisciplineServiceClient client;
    public async Task<bool> IsDisciplineExists(string id)
    {
        CheckRequest request = new CheckRequest() { Id = id };
        CheckResponse check;
        try
        {
            check = await client.CheckDisciplineAsync(request);
        }
        catch (Exception ex)
        {
            throw new GrpcException(ErrorName.DisciplineServiceNotWork);
        }
        return check.IsExists;
    }

        
}
