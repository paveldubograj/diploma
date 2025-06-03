using System;

namespace UserService.BusinessLogic.Services.Interfaces;

public interface IFileStorageConfig
{
    string WebRootPath { get; }
}
