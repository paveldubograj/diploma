using System;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using UserService.API.Protos;
using UserService.BusinessLogic.Services.Interfaces;

namespace UserService.API.Services;

public class TournamentGrpcService : TournamentRegisterService.TournamentRegisterServiceBase
{
    private readonly IUserManageService _userManageService;
    public TournamentGrpcService(IUserManageService userManageService)
    {
        _userManageService = userManageService;
    }
    public override async Task<Empty> AddToUser(Request request, ServerCallContext context)
    {
        await _userManageService.RegisterForTournamentAsync(request.UserId, request.TournamentId);
        return new Empty();
    }

    public override async Task<Empty> RemoveFromUser(Request request, ServerCallContext context)
    {
        await _userManageService.RemoveUserTournamentAsync(request.UserId, request.TournamentId);
        return new Empty();
    }
}
