using System.Diagnostics;
using System.Text.Json;
using Microsoft.ApplicationInsights;
using Polly;
using Polly.CircuitBreaker;

namespace StudentApi.WebApi.Middleware;

public sealed class PollyCircuitBreakerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PollyCircuitBreakerMiddleware> _logger;
    private readonly TelemetryClient _telemetryClient;
    private readonly ResiliencePipeline _pipeline;

    public PollyCircuitBreakerMiddleware(
        RequestDelegate next,
        ILogger<PollyCircuitBreakerMiddleware> logger,
        TelemetryClient telemetryClient,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _telemetryClient = telemetryClient;

        var failureRatio = configuration.GetValue<double?>("Polly:CircuitBreaker:FailureRatio") ?? 0.5;
        var minimumThroughput = configuration.GetValue<int?>("Polly:CircuitBreaker:MinimumThroughput") ?? 10;
        var samplingDurationSeconds = configuration.GetValue<int?>("Polly:CircuitBreaker:SamplingDurationSeconds") ?? 30;
        var breakDurationSeconds = configuration.GetValue<int?>("Polly:CircuitBreaker:BreakDurationSeconds") ?? 20;

        _pipeline = new ResiliencePipelineBuilder()
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<Exception>(),
                FailureRatio = failureRatio,
                MinimumThroughput = minimumThroughput,
                SamplingDuration = TimeSpan.FromSeconds(samplingDurationSeconds),
                BreakDuration = TimeSpan.FromSeconds(breakDurationSeconds),
                OnOpened = _ =>
                {
                    _logger.LogWarning(
                        "Polly circuit breaker opened. FailureRatio: {FailureRatio}, MinimumThroughput: {MinimumThroughput}, SamplingDurationSeconds: {SamplingDurationSeconds}, BreakDurationSeconds: {BreakDurationSeconds}",
                        failureRatio,
                        minimumThroughput,
                        samplingDurationSeconds,
                        breakDurationSeconds);

                    _telemetryClient.TrackTrace("Polly circuit breaker opened", new Dictionary<string, string>
                    {
                        ["FailureRatio"] = failureRatio.ToString("0.##"),
                        ["MinimumThroughput"] = minimumThroughput.ToString(),
                        ["SamplingDurationSeconds"] = samplingDurationSeconds.ToString(),
                        ["BreakDurationSeconds"] = breakDurationSeconds.ToString(),
                        ["EventType"] = "CircuitOpened"
                    });

                    return default;
                },
                OnClosed = _ =>
                {
                    _logger.LogInformation("Polly circuit breaker closed.");

                    _telemetryClient.TrackTrace("Polly circuit breaker closed", new Dictionary<string, string>
                    {
                        ["EventType"] = "CircuitClosed"
                    });

                    return default;
                },
                OnHalfOpened = _ =>
                {
                    _logger.LogInformation("Polly circuit breaker half-opened. A trial request will be allowed.");

                    _telemetryClient.TrackTrace("Polly circuit breaker half-opened", new Dictionary<string, string>
                    {
                        ["EventType"] = "CircuitHalfOpened"
                    });

                    return default;
                }
            })
            .Build();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;

        try
        {
            await _pipeline.ExecuteAsync(async cancellationToken =>
            {
                context.RequestAborted = cancellationToken;
                await _next(context);
            }, context.RequestAborted);
        }
        catch (BrokenCircuitException)
        {
            _logger.LogWarning(
                "Request rejected because Polly circuit breaker is open. TraceId: {TraceId}, Method: {Method}, Path: {Path}",
                traceId,
                context.Request.Method,
                context.Request.Path);

            _telemetryClient.TrackTrace("Request rejected while circuit is open", new Dictionary<string, string>
            {
                ["TraceId"] = traceId,
                ["Method"] = context.Request.Method,
                ["Path"] = context.Request.Path,
                ["EventType"] = "CircuitOpenRequestRejected"
            });

            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            context.Response.ContentType = "application/json";

            var payload = new
            {
                message = "Service temporarily unavailable. Please retry shortly.",
                traceId
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}