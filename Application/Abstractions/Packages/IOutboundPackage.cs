namespace Application.Abstractions.Package;

public interface IOutboundPackage
{ 
    public abstract byte[] GetOutboundBytes();
}
