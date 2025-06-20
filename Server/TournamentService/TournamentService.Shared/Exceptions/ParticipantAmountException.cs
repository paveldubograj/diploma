using System;

namespace TournamentService.Shared.Exceptions;

public class ParticipantAmountException : Exception
{
    public ParticipantAmountException() { }
    public ParticipantAmountException(string message) : base(message) { }
    public ParticipantAmountException(string message, Exception innerException) : base(message, innerException) { }
}
