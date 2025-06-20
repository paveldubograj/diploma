using System;
using Microsoft.AspNetCore.Http;

namespace NewsService.BusinessLogic.Services.Interfaces;

public interface IImageService
{
    public Task<string> SaveImage(IFormFile file, string newsId);
    public bool DeleteImage(string relativePath);
}
