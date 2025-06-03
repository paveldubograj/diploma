using System;

namespace NewsService.BusinessLogic.Services.Interfaces;

public interface IFileStorageConfig
{
    string WebRootPath { get; }
}

