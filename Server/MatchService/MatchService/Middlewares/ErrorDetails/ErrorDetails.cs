using System;

namespace MatchService.API.Middlewares.ErrorDetails;

public class ErrorDetails
{
    public int StatusCode { get; set; }
    public string Title { get; set; }
}
