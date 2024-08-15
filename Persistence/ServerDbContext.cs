namespace Persistence;

internal class ServerDbContext : DbContext, IServerDbContext
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public ServerDbContext(
        DbContextOptions<ServerDbContext> options,
        IDateTimeProvider dateTimeProvider)
        :base(options)
    {
        _dateTimeProvider = dateTimeProvider;

        Database.Migrate();
        ChangeTracker.StateChanged += UpdateTimestamps;
        ChangeTracker.Tracked += UpdateTimestamps;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    private void UpdateTimestamps(object sender, EntityEntryEventArgs eventArg)
    {
        if (eventArg.Entry.Entity is not BaseEntity baseEntity)
        {
            return;
        }

        switch (eventArg.Entry.State)
        {
            case EntityState.Modified:
                baseEntity.UpdatedUtc = _dateTimeProvider.CurrentDateTimeUtc;
                break;
            case EntityState.Added:
                baseEntity.CreatedUtc = _dateTimeProvider.CurrentDateTimeUtc;
                baseEntity.UpdatedUtc = _dateTimeProvider.CurrentDateTimeUtc;
                break;
            case EntityState.Detached:
            case EntityState.Unchanged:
            case EntityState.Deleted:
            default:
                break;
        }
    }
}
