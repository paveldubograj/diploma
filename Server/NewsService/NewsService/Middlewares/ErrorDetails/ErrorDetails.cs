using System;

namespace NewsService.API.Middlewares.ErrorDetails;

public class ErrorDetails
{
    public int StatusCode { get; set; }
    public string Title { get; set; }
}
