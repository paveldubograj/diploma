using System;

namespace TournamentService.Shared.Constants;

public class ErrorName
{
    public static string MatchNotFound => "Match Not Found";
    public static string TournamentNotFound => "Tournament Not Found";
    public static string ParticipantNotFound => "Participant Not Found";
    public static string YouAreNotAllowed => "You Are Not Allowed To Do This";
    public static string ProvidedIdIsNull => "Provided Id Is Null Or Empty";
    public static string ProvidedNameIsNull => "Provided Name Is Null Or Empty";
    public static string ProvidedMatchIsNull => "Provided Match Is Null";
    public static string WrongTournamentOperationCall => "You cannot do it with this tournament";
}
