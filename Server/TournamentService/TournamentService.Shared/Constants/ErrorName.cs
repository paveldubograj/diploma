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
    public static string NotEnoughParticipants => "Not Enough Participants";
    public static string MaxParticipants => "There are already max participants in tournament";
    public static string RegistrationNotAllowed => "Registration For This Tournament Is Not Allowed";
    public static string AlreadyEnded => "Tournament Already Ended";
    public static string AlreadyStarted => "Tournament Already Started";
    public static string WrongImageFormat => "This Image Format Is Not Allowed";
    public static string ImageTooLarge => "This Image Is Too Large";
    public static string ImageTooSmall => "This Image Is Small";
    public static string DisciplineNotFound => "Provided Discipline Not Found";
}
