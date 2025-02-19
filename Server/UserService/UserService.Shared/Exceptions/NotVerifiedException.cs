using System;

namespace UserService.Shared.Exceptions;

public class NotVerifiedException : Exception
{
    public NotVerifiedException() { }
    public NotVerifiedException(string message) : base(message) { }
    public NotVerifiedException(string message, Exception innerException) : base(message, innerException) { }
}
