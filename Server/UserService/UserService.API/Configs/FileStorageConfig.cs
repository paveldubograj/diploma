using System;
using UserService.BusinessLogic.Services.Interfaces;

namespace UserService.API.Configs;

public class FileStorageConfig : IFileStorageConfig
{
    public string WebRootPath { get; }
    public FileStorageConfig(IWebHostEnvironment env)
    {
        WebRootPath = env.WebRootPath;
    }
}
