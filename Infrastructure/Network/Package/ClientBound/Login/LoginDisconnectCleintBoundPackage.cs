namespace Infrastructure.Network.Package.ClientBound.Login;

public class LoginDisconnectCleintBoundPackage
    (TextComponent reason)
        : ClientBoundPackage(ProtocolDefinition.LoginDisconnectPackageId)
{
    public override async Task<bool> RespondAsync(IConnection connection, CancellationToken cancellationToken = default)
    {
        var reasonTextComponentJson = JsonConvert.SerializeObject(reason);

        await connection.Stream
            .WriteVarInt(Id)
            .WriteString(reasonTextComponentJson)
            .SendAsync(cancellationToken);

        return true;
    }
}
