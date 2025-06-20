using System;

namespace TournamentService.Shared.Exceptions;

public class GrpcException : Exception
{
    public GrpcException() { }
    public GrpcException(string message) : base(message) { }
    public GrpcException(string message, Exception innerException) : base(message, innerException) { }
}
