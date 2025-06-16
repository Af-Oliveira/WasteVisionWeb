using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DDDSample1.Application.Services;

public class TimedBackgroundService : BackgroundService
{
    private readonly ILogger<TimedBackgroundService> _logger;
    private readonly PeriodicTimer _timer;
    private readonly IServiceScopeFactory _scopeFactory;

    public TimedBackgroundService(
        ILogger<TimedBackgroundService> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (await _timer.WaitForNextTickAsync(stoppingToken))
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Timed Background Service is stopping.");
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _timer.Dispose();
        await base.StopAsync(stoppingToken);
    }
}