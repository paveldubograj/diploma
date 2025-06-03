using System;

namespace UserService.Shared.Contants;

public static class ErrorName
{
    public static string UserNotFound => "User Not Found";
    public static string UserAlreadyExist => "User Already Exist";
    public static string EmailNotFound => "Email Not Found";
    public static string PasswordInvalid => "Password Is Invalid";
    public static string RoleNotFound => "Role Not Found";
    public static string WrongImageFormat => "This Image Format Is Not Allowed";
    public static string ImageTooLarge => "This Image Is Too Large";
    public static string ImageTooSmall => "This Image Is Small";
}
