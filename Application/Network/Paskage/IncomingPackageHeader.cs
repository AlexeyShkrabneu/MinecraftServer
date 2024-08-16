namespace Application.Network.Paskage;

public class IncomingPackageHeader(int id, int length) : PackageBase(id)
{
    public readonly int Length = length;
}
