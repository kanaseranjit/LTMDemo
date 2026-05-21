using System.Diagnostics;
using System.Text;
using Microsoft.ApplicationInsights;

namespace StudentApi.WebApi.Middleware;

public sealed class RequestResponseTelemetryMiddleware
{
    private readonly RequestDelegate _next;
    private readonly TelemetryClient _telemetryClient;
    private readonly ILogger<RequestResponseTelemetryMiddleware> _logger;
    private readonly IConfiguration _configuration;

    public RequestResponseTelemetryMiddleware(
        RequestDelegate next,
        TelemetryClient telemetryClient,
        ILogger<RequestResponseTelemetryMiddleware> logger,
        IConfiguration configuration)
    {
        _next = next;
        _telemetryClient = telemetryClient;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
        var requestBody = await ReadRequestBodyAsync(context.Request);
        var originalResponseBody = context.Response.Body;

        await using var responseBodyBuffer = new MemoryStream();
        context.Response.Body = responseBodyBuffer;

        try
        {
            await _next(context);

            stopwatch.Stop();
            var responseBody = await ReadResponseBodyAsync(context.Response);

            LogRequest(context, traceId, requestBody);
            LogResponse(context, traceId, responseBody, stopwatch.ElapsedMilliseconds);

            await responseBodyBuffer.CopyToAsync(originalResponseBody);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            LogException(context, traceId, requestBody, ex, stopwatch.ElapsedMilliseconds);
            throw;
        }
        finally
        {
            context.Response.Body = originalResponseBody;
        }
    }

    private async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        request.EnableBuffering();
        request.Body.Position = 0;

        using var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;

        return Truncate(body);
    }

    private async Task<string> ReadResponseBodyAsync(HttpResponse response)
    {
        response.Body.Position = 0;
        using var reader = new StreamReader(response.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        response.Body.Position = 0;
        return Truncate(body);
    }

    private void LogRequest(HttpContext context, string traceId, string requestBody)
    {
        var properties = new Dictionary<string, string>
        {
            ["TraceId"] = traceId,
            ["Method"] = context.Request.Method,
            ["Path"] = context.Request.Path,
            ["QueryString"] = context.Request.QueryString.ToString(),
            ["RequestBody"] = requestBody,
            ["EventType"] = "Request"
        };

        _telemetryClient.TrackTrace("Incoming request", properties);
        _logger.LogInformation(
            "Incoming request. TraceId: {TraceId}, Method: {Method}, Path: {Path}, RequestBody: {RequestBody}",
            traceId,
            context.Request.Method,
            context.Request.Path,
            requestBody);
    }

    private void LogResponse(HttpContext context, string traceId, string responseBody, long elapsedMilliseconds)
    {
        var properties = new Dictionary<string, string>
        {
            ["TraceId"] = traceId,
            ["Method"] = context.Request.Method,
            ["Path"] = context.Request.Path,
            ["StatusCode"] = context.Response.StatusCode.ToString(),
            ["ElapsedMs"] = elapsedMilliseconds.ToString(),
            ["ResponseBody"] = responseBody,
            ["EventType"] = "Response"
        };

        _telemetryClient.TrackTrace("Outgoing response", properties);
        _logger.LogInformation(
            "Outgoing response. TraceId: {TraceId}, Method: {Method}, Path: {Path}, StatusCode: {StatusCode}, ElapsedMs: {ElapsedMs}, ResponseBody: {ResponseBody}",
            traceId,
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            elapsedMilliseconds,
            responseBody);
    }

    private void LogException(HttpContext context, string traceId, string requestBody, Exception ex, long elapsedMilliseconds)
    {
        var properties = new Dictionary<string, string>
        {
            ["TraceId"] = traceId,
            ["Method"] = context.Request.Method,
            ["Path"] = context.Request.Path,
            ["QueryString"] = context.Request.QueryString.ToString(),
            ["RequestBody"] = requestBody,
            ["ElapsedMs"] = elapsedMilliseconds.ToString(),
            ["EventType"] = "Exception"
        };

        _telemetryClient.TrackException(ex, properties);
        _logger.LogError(
            ex,
            "Unhandled exception. TraceId: {TraceId}, Method: {Method}, Path: {Path}, ElapsedMs: {ElapsedMs}, RequestBody: {RequestBody}",
            traceId,
            context.Request.Method,
            context.Request.Path,
            elapsedMilliseconds,
            requestBody);
    }

    private string Truncate(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var maxBodyLength = _configuration.GetValue<int?>("TelemetryLogging:MaxBodyLength") ?? 4096;
        if (value.Length <= maxBodyLength)
        {
            return value;
        }

        return value[..maxBodyLength] + "...(truncated)";
    }
}