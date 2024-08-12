﻿using Finance.Domain.Abstracts;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;


namespace Finance.Application.Abstractions.Behaviors;


internal class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var name = request.GetType().Name;

        try
        {
            logger.LogInformation("Executing request {Request}", name);

            var result = await next();

            if (result.IsSuccess)
            {
                logger.LogInformation("Request {Request} processed successfully", name);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    logger.LogError("Request {Request} processed with error", name);
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Request {Request} processing failed", name);
            throw;
        }
    }
}
