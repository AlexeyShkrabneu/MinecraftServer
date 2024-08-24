namespace Domain.Constants;

public static class ProtocolDefinition
{
    public const int HandshakePackageId = 0x00;
    
    #region Status
    public const int StatusPackageId = 0x00;
    public const int PingPackageId = 0x01;
    #endregion

    #region Login
    public const string ServerId = "";
    public const int LoginStart = 0x00;
    public const int LoginSuccessPackageId = 0x02;
    public const int LoginDisconnectPackageId = 0x00;
    public const int LoginAcknowledgedPackageId = 0x03;
    public const int EncryptionRequestPackageId = 0x01;
    public const int EncryptionResponsePackageId = 0x01;
    public const int LoginPlayResponsePackageId = 0x2B;

    public const bool CleintStrictErrorHandling = false;
    #endregion

    #region Configuration
    public const int ConfigurationDisconnectPackageId = 0x02;
    #endregion

    #region Play
    public const int PlayDisconnectPackageId = 0x1D;

    #endregion
}
