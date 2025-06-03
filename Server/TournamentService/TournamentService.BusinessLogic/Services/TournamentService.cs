using System;
using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using TournamentService.BusinessLogic.Models.Filters;
using TournamentService.BusinessLogic.Models.Match;
using TournamentService.BusinessLogic.Models.Tournament;
using TournamentService.BusinessLogic.Services.Interfaces;
using TournamentService.BusinessLogic.Services.Tournaments;
using TournamentService.BusinessLogic.Services.Tournaments.Interfaces;
using TournamentService.DataAccess.Entities;
using TournamentService.DataAccess.Repositories.Interfaces;
using TournamentService.DataAccess.Specifications;
using TournamentService.Shared.Constants;
using TournamentService.Shared.Enums;
using TournamentService.Shared.Exceptions;
using TournamentService.Shared.Options;

namespace TournamentService.BusinessLogic.Services;

public class TournamentService : ITournamentService
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IParticipantRepository _participantRepository;
    private readonly IImageService _imageService;
    private readonly IDisciplineGrpcService _disciplineService;
    private readonly ISingleEliminationBracket _singleEliminationBracket;
    private readonly IRoundRobinBracket _roundRobinBracket;
    private readonly IDoubleEliminationBracket _doubleEliminationBracket;
    private readonly ISwissBracket _swissBracket;
    private readonly IMapper _mapper;
    public TournamentService(
        ITournamentRepository tournamentRepository,
        IParticipantRepository participantRepository,
        IImageService imageService,
        IDisciplineGrpcService disciplineService,
        IMapper mapper,
        ISingleEliminationBracket singleEliminationBracket,
        IRoundRobinBracket roundRobinBracket,
        ISwissBracket swissBracket,
        IDoubleEliminationBracket doubleEliminationBracket)
    {
        _tournamentRepository = tournamentRepository;
        _participantRepository = participantRepository;
        _imageService = imageService;
        _disciplineService = disciplineService;
        _mapper = mapper;
        _singleEliminationBracket = singleEliminationBracket;
        _roundRobinBracket = roundRobinBracket;
        _swissBracket = swissBracket;
        _doubleEliminationBracket = doubleEliminationBracket;
    }
    public async Task<TournamentDto> AddAsync(TournamentCreateDto newsDto, string ownerId)
    {
        var tournament = _mapper.Map<Tournament>(newsDto);
        if (!await _disciplineService.IsDisciplineExists(tournament.DisciplineId)) throw new NotFoundException(ErrorName.DisciplineNotFound);
        tournament.OwnerId = ownerId;
        tournament.Status = TournamentStatus.Pending;
        tournament.Id = Guid.NewGuid().ToString();
        var result = await _tournamentRepository.AddAsync(tournament);
        return _mapper.Map<TournamentDto>(result);
    }

    public async Task<TournamentDto> DeleteAsync(string id, string userId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(id);
        if (tournament == null)
        {
            throw new NotFoundException(ErrorName.TournamentNotFound);
        }
        if (!tournament.OwnerId.Equals(userId))
        {
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        var result = _tournamentRepository.DeleteAsync(tournament);
        return _mapper.Map<TournamentDto>(result);
    }

    public async Task<List<TournamentCleanDto>> GetAllByPageAsync(int page, int pageSize)
    {
        var list = await _tournamentRepository.GetAsync(page, pageSize);
        return _mapper.Map<List<TournamentCleanDto>>(list);
    }

    public async Task<List<TournamentCleanDto>> GetByFilterAsync(TournamentFilter filter, TournamentSortOptions? options, int page, int pageSize)
    {
        TournamentSpecification spec = TournamentSpecification.FilterTournaments(filter.SearchString, filter.CategoryId, filter.Status, filter.Format, filter.StartTime, filter.EndTime);
        var list = await _tournamentRepository.GetBySpecificationAsync(spec, options, page, pageSize);
        return _mapper.Map<List<TournamentCleanDto>>(list);
    }

    public async Task<TournamentDto> GetByIdAsync(string id)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(id);
        if (tournament == null)
        {
            throw new NotFoundException(ErrorName.TournamentNotFound);
        }
        return _mapper.Map<TournamentDto>(tournament);
    }
    public async Task<TournamentDto> UpdateAsync(string id, TournamentDto newsDto, string userId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(id);
        if (tournament == null)
        {
            throw new NotFoundException(ErrorName.TournamentNotFound);
        }
        if (!tournament.OwnerId.Equals(userId))
        {
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        if (!await _disciplineService.IsDisciplineExists(tournament.DisciplineId)) throw new NotFoundException(ErrorName.DisciplineNotFound);
        var newsUp = _mapper.Map(newsDto, tournament);
        var res = _tournamentRepository.UpdateAsync(newsUp);
        return _mapper.Map<TournamentDto>(res);
    }

    public async void SetNextRound(string id, string userId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(id);
        if (tournament == null)
        {
            throw new NotFoundException(ErrorName.TournamentNotFound);
        }
        if (!tournament.OwnerId.Equals(userId))
        {
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        if (tournament.Status == TournamentStatus.Completed || tournament.Status == TournamentStatus.Cancelled)
        {
            throw new WrongCallException(ErrorName.AlreadyEnded);
        }
        switch (tournament.Format)
        {
            case TournamentFormat.Swiss:
                await _swissBracket.GenerateSwissMatches(id);
                break;
            case TournamentFormat.DoubleElimination:
                await _doubleEliminationBracket.GenerateLowerBracket(id);
                break;
            default: throw new WrongCallException(ErrorName.WrongTournamentOperationCall);
        }
    }
    public async void SetWinnerForMatchAsync(string tournamentId, string matchId, string winnerId, string looserId, int winPoints, int loosePoints, string userId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (tournament is null)
        {
            throw new NotFoundException(ErrorName.TournamentNotFound);
        }
        if (!tournament.OwnerId.Equals(userId))
        {
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        if (tournament.Status == TournamentStatus.Completed || tournament.Status == TournamentStatus.Cancelled)
        {
            throw new WrongCallException(ErrorName.AlreadyEnded);
        }
        switch (tournament.Format)
        {
            case TournamentFormat.Swiss:
                await _swissBracket.HandleMatchResult(matchId, winnerId, looserId, winPoints, loosePoints);
                break;
            case TournamentFormat.SingleElimination:
                await _singleEliminationBracket.HandleMatchResult(matchId, winnerId, looserId, winPoints, loosePoints);
                break;
            case TournamentFormat.RoundRobin:
                await _roundRobinBracket.HandleMatchResult(matchId, winnerId, winPoints, loosePoints);
                break;
            case TournamentFormat.DoubleElimination:
                await _doubleEliminationBracket.HandleMatchResult(matchId, winnerId, looserId, winPoints, loosePoints);
                break;
            default: throw new WrongCallException(ErrorName.WrongTournamentOperationCall);
        }
    }
    public async Task<TournamentCleanDto> StartTournamentAsync(string tournamentId, string userId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (tournament == null)
        {
            throw new NotFoundException(ErrorName.TournamentNotFound);
        }
        if (!tournament.OwnerId.Equals(userId))
        {
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        if (tournament.Status == TournamentStatus.Completed || tournament.Status == TournamentStatus.Cancelled)
        {
            throw new WrongCallException(ErrorName.AlreadyEnded);
        }
        if (tournament.Status == TournamentStatus.Ongoing)
        {
            throw new WrongCallException(ErrorName.AlreadyStarted);
        }
        tournament.Status = TournamentStatus.Ongoing;
        GenerateBracketAsync(tournamentId, userId);
        await _tournamentRepository.UpdateAsync(tournament);
        return _mapper.Map<TournamentCleanDto>(tournament);
    }
    public async Task<TournamentCleanDto> EndTournamentAsync(string tournamentId, string userId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (tournament == null)
        {
            throw new NotFoundException(ErrorName.TournamentNotFound);
        }
        if (!tournament.OwnerId.Equals(userId))
        {
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        if (tournament.Status == TournamentStatus.Completed || tournament.Status == TournamentStatus.Cancelled)
        {
            throw new WrongCallException(ErrorName.AlreadyEnded);
        }
        tournament.Status = TournamentStatus.Completed;
        var participamts = await _participantRepository.GetAllAsync(tournamentId);
        participamts = participamts.OrderByDescending(p => p.Points).ToList();
        tournament.WinnerId = participamts[0].Id;
        await _tournamentRepository.UpdateAsync(tournament);
        return _mapper.Map<TournamentCleanDto>(tournament);
    }
    public async void GenerateBracketAsync(string id, string userId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(id);
        if (tournament == null)
        {
            throw new NotFoundException(ErrorName.TournamentNotFound);
        }
        if (!tournament.OwnerId.Equals(userId))
        {
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        if (tournament.Status == TournamentStatus.Completed || tournament.Status == TournamentStatus.Cancelled)
        {
            throw new WrongCallException(ErrorName.AlreadyEnded);
        }
        switch (tournament.Format)
        {
            case TournamentFormat.Swiss:
                SetNextRound(id, userId);
                break;
            case TournamentFormat.SingleElimination:
                await _singleEliminationBracket.GenerateBracket(id);
                break;
            case TournamentFormat.RoundRobin:
                await _roundRobinBracket.GenerateBracket(id);
                break;
            case TournamentFormat.DoubleElimination:
                await _doubleEliminationBracket.GenerateBracket(id);
                break;
            default: throw new WrongCallException(ErrorName.WrongTournamentOperationCall);
        }
    }

    public async Task<int> GetTotalAsync()
    {
        return await _tournamentRepository.GetTotalAsync();
    }
    public async Task<TournamentDto> AddImageAsync(string id, IFormFile file, string userId)
    {
        var news = await _tournamentRepository.GetByIdAsync(id);
        if (news == null)
        {
            throw new NotFoundException(ErrorName.TournamentNotFound);
        }
        if (!news.OwnerId.Equals(userId))
        {
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        news.ImagePath = await _imageService.SaveImage(file, id);
        var res = await _tournamentRepository.UpdateAsync(news);
        return _mapper.Map<TournamentDto>(res);
    }

    public async Task<TournamentDto> RemoveImageAsync(string id, string userId)
    {
        var news = await _tournamentRepository.GetByIdAsync(id);
        if (news == null)
        {
            throw new NotFoundException(ErrorName.TournamentNotFound);
        }
        if (!news.OwnerId.Equals(userId))
        {
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        _imageService.DeleteImage(id);
        news.ImagePath = string.Empty;
        var res = await _tournamentRepository.UpdateAsync(news);
        return _mapper.Map<TournamentDto>(res);
    }

    public async Task<List<TournamentCleanDto>> GetByOwnerAsync(string ownerId, int page, int pageSize)
    {
        TournamentSpecification spec = new TournamentSpecification(c => c.OwnerId.Equals(ownerId));
        var list = await _tournamentRepository.GetBySpecificationAsync(spec, null, page, pageSize);
        return _mapper.Map<List<TournamentCleanDto>>(list);
    }
}
