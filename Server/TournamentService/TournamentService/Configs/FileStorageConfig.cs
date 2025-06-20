using System;
using TournamentService.BusinessLogic.Services.Interfaces;

namespace TournamentService.API.Configs;

public class FileStorageConfig : IFileStorageConfig
{
    public string WebRootPath { get; }
    public FileStorageConfig(IWebHostEnvironment env)
    {
        WebRootPath = env.WebRootPath;
    }
}

