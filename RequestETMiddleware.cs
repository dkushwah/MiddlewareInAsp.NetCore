


using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public class RequestExecutionTime
{
    private readonly ILogger _logger;

    public RequestExecutionTime(RequestDelegate requestDelegate, ILogger logger)
    {
        _next = requestDelegate;
        this._logger = logger;
    }

    public RequestDelegate _next { get; }

    public async Task Invoke(HttpContext httpContext)
    {

        var watch = new Stopwatch();
        watch.Start();
        await _next.Invoke(httpContext);
        watch.Stop();
        var logmsg = "Total Request Time in Milliseconds:" + watch.Elapsed.TotalMilliseconds;
        this._logger.LogInformation(logmsg);
        Console.WriteLine(logmsg);
    }
}