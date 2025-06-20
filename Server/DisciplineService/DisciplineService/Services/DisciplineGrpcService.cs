using System;
using DisciplineService.API.Protos;
using DisciplineService.BusinessLogic.Services.Interfaces;
using DisciplineService.Shared.Exceptions;
using Grpc.Core;

namespace DisciplineService.API.Services;

public class DisciplineService : Protos.DisciplineService.DisciplineServiceBase
{
    private readonly IDisciplineService _disciplineService;
    public DisciplineService(IDisciplineService disciplineService)
    {
        _disciplineService = disciplineService;
    }

    public override async Task<CheckResponse> CheckDiscipline(CheckRequest request, ServerCallContext context)
    {
        try
        {
            var p = await _disciplineService.GetByIdAsync(request.Id);
        }
        catch (NotFoundException)
        {
            return new CheckResponse() { IsExists = false };
        }
        return new CheckResponse() { IsExists = true };
    }
}
