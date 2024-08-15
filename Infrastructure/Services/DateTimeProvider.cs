namespace Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime CurrentDateTimeUtc => DateTime.UtcNow;

    public long CurrentUnixDateTime => TransformToUnixDateTime(CurrentDateTimeUtc);

    public long TransformToUnixDateTime(DateTime dateTimeUtc)
    {
        return ((DateTimeOffset)dateTimeUtc).ToUnixTimeSeconds();
    }
}
