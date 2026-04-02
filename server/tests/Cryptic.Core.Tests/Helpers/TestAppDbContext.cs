using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Moq;

namespace Cryptic.Core.Tests.Helpers;

internal sealed class TestAppDbContext : AppDbContext
{
    public TestAppDbContext() : base(
        Mock.Of<IConfiguration>(),
        Mock.Of<IHostEnvironment>(e => e.EnvironmentName == "Production"),
        new DbContextOptionsBuilder<AppDbContext>().Options)
    { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
    }
}