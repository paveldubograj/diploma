using System;
using NewsService.BusinessLogic.Services.Interfaces;

namespace NewsService.API.Configs;

public class FileStorageConfig : IFileStorageConfig
{
    public FileStorageConfig(IWebHostEnvironment env)
    {
        WebRootPath = env.WebRootPath;
    }
    public string WebRootPath { get; }
}

