namespace Infrastructure.Network.Package.ClientBound.Login;

public class LoginSuccessClientBoundPackage()
    : ClientBoundPackage(ProtocolDefinition.LoginSuccessPackageId)
{
    public async override Task<bool> RespondAsync(IConnection connection, CancellationToken cancellationToken = default)
    {
        var playerUUID = BitConverter.ToUInt16(connection.Player.Id.ToByteArray());

        await connection.Stream
            .WriteVarInt(Id)
            .WriteUUID(connection.Player.Id)
            .WriteString(connection.Player.Username)
            .WriteProperties(connection.Player.Properties)
            .WriteBool(true)
            .SendAsync(cancellationToken);
        
        connection.ChangeState(ConnectionState.Play);

        return true;
    }
}
