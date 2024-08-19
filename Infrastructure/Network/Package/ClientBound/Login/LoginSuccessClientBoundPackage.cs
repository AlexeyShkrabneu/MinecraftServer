namespace Infrastructure.Network.Package.ClientBound.Login;

public class LoginSuccessClientBoundPackage()
    : ClientBoundPackage(ProtocolDefinition.LoginSuccessPackageId)
{
    public async override Task<bool> RespondAsync(IConnection connection, CancellationToken cancellationToken = default)
    {
        await connection.Stream
            .WriteVarInt(Id)
            .WriteGuid(connection.PlayerProfile.Id)
            .WriteString(connection.PlayerProfile.Username)
            .WriteProperties(connection.PlayerProfile.Properties)
            .WriteBool(ProtocolDefinition.CleintStrictErrorHandling)
            .SendAsync(cancellationToken);
        
        return true;
    }
}
