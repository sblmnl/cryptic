using Cryptic.Shared.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cryptic.Shared.Features.Notes.Persistence;

public class TimeBasedNoteSelfDestructionService : IHostedService
{
    private readonly AppDbContext _database;
    private readonly ILogger<TimeBasedNoteSelfDestructionService> _logger;

    public TimeBasedNoteSelfDestructionService(
        AppDbContext database,
        ILogger<TimeBasedNoteSelfDestructionService> logger)
    {
        _database = database;
        _logger = logger;
    }
    
    public async Task StartAsync(CancellationToken ct)
    {
        _logger.LogInformation("Service started!");

        while (!ct.IsCancellationRequested)
        {
            _logger.LogInformation("Looking for notes to destroy... ");
            
            try
            {
                var numberOfNotesDestroyed = await _database.Notes
                    .Where(x => x.DeleteAfterTime < DateTimeOffset.Now)
                    .ExecuteDeleteAsync(ct);

                _logger.LogInformation(
                    "Successfully destroyed {NumberOfNotesDestroyed} notes in the database!",
                    numberOfNotesDestroyed);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while communicating with the database!");
            }

            await Task.Delay(TimeSpan.FromMinutes(15), ct);
        }
    }

    public Task StopAsync(CancellationToken ct)
    {
        _logger.LogInformation("Service stopped!");
        return Task.CompletedTask;
    }
}
