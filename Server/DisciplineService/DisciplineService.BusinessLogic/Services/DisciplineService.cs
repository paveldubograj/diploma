using System;
using AutoMapper;
using DisciplineService.BusinessLogic.Models;
using DisciplineService.BusinessLogic.Services.Interfaces;
using DisciplineService.DataAccess.Entities;
using DisciplineService.DataAccess.Repositories.Interfaces;
using DisciplineService.Shared.Constants;
using DisciplineService.Shared.Exceptions;

namespace DisciplineService.BusinessLogic.Services;

public class DisciplineService : IDisciplineService
{
    private readonly IDisciplineRepository _disciplineRepository;
    private readonly IMapper _mapper;
    public DisciplineService(IDisciplineRepository disciplineRepository, IMapper mapper){
        _disciplineRepository = disciplineRepository;
        _mapper = mapper;
    }
    public async Task<DisciplineDto> AddAsync(DisciplineDto disciplineDto)
    {
        var obj = _mapper.Map<Discipline>(disciplineDto);
        var res = await _disciplineRepository.AddAsync(obj);
        return _mapper.Map<DisciplineDto>(res);
    }

    public async Task<DisciplineCleanDto> DeleteAsync(string id)
    {
        var obj = await _disciplineRepository.GetByIdAsync(id);
        if(obj == null){
            throw new NotFoundException(ErrorName.DisciplineNotFound);
        }
        var res = await _disciplineRepository.DeleteAsync(obj);
        return _mapper.Map<DisciplineCleanDto>(res);
    }

    public async Task<List<DisciplineCleanDto>> GetAllAsync()
    {
        var res = await _disciplineRepository.GetAllAsync();
        return _mapper.Map<List<DisciplineCleanDto>>(res);
    }

    public async Task<DisciplineDto> GetByIdAsync(string id)
    {
        var obj = await _disciplineRepository.GetByIdAsync(id);
        if(obj == null){
            throw new NotFoundException(ErrorName.DisciplineNotFound);
        }
        return _mapper.Map<DisciplineDto>(obj);
    }

    public async Task<DisciplineDto> UpdateAsync(string id, DisciplineDto disciplineDto)
    {
        var tag = await _disciplineRepository.GetByIdAsync(id);
        if(tag == null){
            throw new NotFoundException(ErrorName.DisciplineNotFound);
        }
        var newsUp = _mapper.Map(disciplineDto, tag);
        var res = _disciplineRepository.UpdateAsync(newsUp);
        return _mapper.Map<DisciplineDto>(res);
    }
}
