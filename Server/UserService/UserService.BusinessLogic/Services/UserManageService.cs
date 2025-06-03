using System;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using UserService.BusinessLogic.Models.User;
using UserService.BusinessLogic.Services.Interfaces;
using UserService.DataAccess.Repositories.Interfaces;
using UserService.DataAccess.Specifications;
using UserService.Shared.Contants;
using UserService.Shared.Exceptions;

namespace UserService.BusinessLogic.Services;

public class UserManageService : IUserManageService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IImageService _imageService;

    public UserManageService(IUserRepository userRepository, IMapper mapper, IImageService imageService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _imageService = imageService;
    }
    public async Task<UserProfileDto> GetByIdAsync(string id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
        {
            throw new NotFoundException(ErrorName.UserNotFound);
        }

        var userDto = _mapper.Map<UserProfileDto>(user);

        return userDto;
    }
    public async Task<IEnumerable<UserCleanDto>> GetByNameAsync(int page, int pageSize, string? firstName, CancellationToken token = default)
    {
        UserSpecification spec = new UserSpecification(user => true);
        if (!string.IsNullOrEmpty(firstName)) spec = new UserSpecification(user => user.UserName.Contains(firstName));
        var users = await _userRepository.GetBySpecAsync(page, pageSize, spec, token);
        var userDtos = _mapper.Map<List<UserCleanDto>>(users);

        return userDtos;
    }
    public async Task<UserProfileDto> UpdateAsync(string id, UserProfileDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
        {
            throw new NotFoundException(ErrorName.UserNotFound);
        }

        var userUpdated = _mapper.Map(dto, user);
        var result = await _userRepository.UpdateAsync(userUpdated);

        if (!result.Succeeded)
        {
            var errorMessage = string.Join(
                Environment.NewLine,
                result.Errors.Select(exception =>
                    exception.Description
                ));

            throw new IdentityException(errorMessage);
        }

        return _mapper.Map<UserProfileDto>(userUpdated);
    }
    public async Task<UserCleanDto> DeleteAsync(string id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
        {
            throw new NotFoundException(ErrorName.UserNotFound);
        }

        var result = await _userRepository.DeleteAsync(user);

        if (!result.Succeeded)
        {
            var errorMessage = string.Join(
                Environment.NewLine,
                result.Errors.Select(exception =>
                    exception.Description
                ));

            throw new IdentityException(errorMessage);
        }

        return _mapper.Map<UserCleanDto>(user);
    }
    public async Task<bool> IsUserExits(string id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        return user != null;
    }

    public async Task<int> GetTotalAsync()
    {
        return await _userRepository.GetTotalAsync();
    }
    public async Task<UserProfileDto> AddImageAsync(string id, IFormFile file)
    {
        var news = await _userRepository.GetByIdAsync(id);
        if (news == null)
        {
            throw new NotFoundException(ErrorName.UserNotFound);
        }
        news.Image = await _imageService.SaveImage(file, id);
        var res = await _userRepository.UpdateAsync(news);
        return _mapper.Map<UserProfileDto>(res);
    }
    public async Task<UserProfileDto> RemoveImageAsync(string id)
    {
        var news = await _userRepository.GetByIdAsync(id);
        if (news == null)
        {
            throw new NotFoundException(ErrorName.UserNotFound);
        }
        _imageService.DeleteImage(id);
        var res = await _userRepository.UpdateAsync(news);
        return _mapper.Map<UserProfileDto>(res);
    }

    public async Task<UserProfileDto> RegisterForTournamentAsync(string userId, string tournamentId)
    {
        var news = await _userRepository.GetByIdAsync(userId);
        if (news == null)
        {
            throw new NotFoundException(ErrorName.UserNotFound);
        }
        return _mapper.Map<UserProfileDto>(await _userRepository.AddUserTournament(news, tournamentId));
    }
    public async Task<UserProfileDto> RemoveUserTournamentAsync(string userId, string tournamentId)
    {
        var news = await _userRepository.GetByIdAsync(userId);
        if (news == null)
        {
            throw new NotFoundException(ErrorName.UserNotFound);
        }
        return _mapper.Map<UserProfileDto>(await _userRepository.RemoveUserTournament(news, tournamentId));
    }
}
