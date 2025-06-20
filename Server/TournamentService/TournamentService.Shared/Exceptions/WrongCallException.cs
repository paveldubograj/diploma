using System;

namespace TournamentService.Shared.Exceptions;

public class WrongCallException : Exception
{
    public WrongCallException() { }
    public WrongCallException(string message) : base(message) { }
    public WrongCallException(string message, Exception innerException) : base(message, innerException) { }
}
