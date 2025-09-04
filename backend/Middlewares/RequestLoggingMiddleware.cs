using System.Diagnostics;
using System.Text;

namespace LocalSearcher.Api.Middlewares;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var start = Stopwatch.GetTimestamp();

        var request = context.Request;
        var method = request.Method;
        var path = request.Path + request.QueryString;
        
        logger.LogInformation("Incoming request: {Method} {Path}", method, path);

        if (method is "POST" or "PUT")
        {
            request.EnableBuffering();
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;

            if (!string.IsNullOrWhiteSpace(body))
            {
                logger.LogInformation("Body: {Body}", body);
            }
        }

        await next(context);

        logger.LogInformation("{Method} {Path} with response code {RespMethod} in {ElapsedMs}ms",
            method, path, context.Response.StatusCode, Stopwatch.GetElapsedTime(start));
    }
}