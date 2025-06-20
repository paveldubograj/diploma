using System;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using NewsService.BusinessLogic.Protos;
using NewsService.BusinessLogic.Services.Interfaces;
using NewsService.Shared.Options;

namespace NewsService.BusinessLogic.Services;

public class DisciplineService : IDisciplineService
{
    public DisciplineService(IOptions<GrpcDisciplineSettings> options)
    {
        client = new Protos.DisciplineService.DisciplineServiceClient(GrpcChannel.ForAddress(options.Value.Address, new GrpcChannelOptions
        {
            HttpHandler = new HttpClientHandler()
        }));
    }
    private readonly Protos.DisciplineService.DisciplineServiceClient client;
    public async Task<bool> IsDisciplineExists(string id)
    {
        CheckRequest request = new CheckRequest(){Id = id};
        return (await client.CheckDisciplineAsync(request)).IsExists;
    }
}
