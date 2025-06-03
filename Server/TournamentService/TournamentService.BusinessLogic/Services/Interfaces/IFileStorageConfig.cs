using System;

namespace TournamentService.BusinessLogic.Services.Interfaces;

public interface IFileStorageConfig
{
    string WebRootPath { get; }
}
