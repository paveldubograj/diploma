using System;
using System.ComponentModel.DataAnnotations;
using TournamentService.Shared.Exceptions;

namespace TournamentService.API.Middlewares;

public class ExceptionAndLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionAndLoggingMiddleware> _logger;

    public ExceptionAndLoggingMiddleware(RequestDelegate next, ILogger<ExceptionAndLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext httpContext)
    {
        _logger.LogInformation($"Request: {httpContext.Request.Method} {httpContext.Request.Path}");
        
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
        
        _logger.LogInformation($"Response: {httpContext.Response.StatusCode} {httpContext.Response}");
    }
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var result = new ErrorDetails.ErrorDetails
        {
            StatusCode = 500,
            Title = exception.Message
        };

        switch (exception)
        {
            case ValidationException :
                result.StatusCode = 400;
                break;
            
            case NotFoundException  :
                result.StatusCode = 404;
                break;

            case BadAuthorizeException :
                result.StatusCode = 400;
                break;

            case WrongCallException :
                result.StatusCode = 400;
                break;

        }
        
        context.Response.StatusCode = result.StatusCode;
        
        _logger.LogError($"Errors: {result.Title}");

        await context.Response.WriteAsync(result.Title);
    }
}
