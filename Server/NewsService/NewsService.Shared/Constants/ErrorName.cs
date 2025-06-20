using System;

namespace NewsService.Shared.Constants;

public class ErrorName
{
    public static string NewsNotFound => "Piece Of News Not Found";
    public static string TagNotFound => "Tag Not Found";
    public static string YouAreNotAllowed => "You Are Not Allowed To Do This";
    public static string WrongImageFormat => "This Image Format Is Not Allowed";
    public static string ImageTooLarge => "This Image Is Too Large";
    public static string ImageTooSmall => "This Image Is Small";
    public static string DisciplineNotFound => "Provided Discipline Not Found";
    public static string DisciplineServiceNotWork => "Discipline Service Not Work";
}
