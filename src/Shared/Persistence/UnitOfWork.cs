namespace Cryptic.Shared.Persistence;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken ct = default);
}

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;
    
    public UnitOfWork(AppDbContext database)
    {
        _db = database;
    }
    
    public Task SaveChangesAsync(CancellationToken ct = default)
    {
        return _db.SaveChangesAsync(ct);
    }
}
