namespace Application.Interfaces.Services;

public interface IDateTimeProvider
{
    DateTime CurrentDateTimeUtc { get; }
    long CurrentUnixDateTime { get; }

    long TransformToUnixDateTime(DateTime dateTimeUtc);
}
