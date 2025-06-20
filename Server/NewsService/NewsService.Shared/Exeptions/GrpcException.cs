using System;

namespace NewsService.Shared.Exeptions;

public class GrpcException : Exception
{
    public GrpcException() { }
    public GrpcException(string message) : base(message) { }
    public GrpcException(string message, Exception innerException) : base(message, innerException) { }
}
