namespace Domain.Models.Encryption;

public class ServerEncryption
{
    public RSA RSA { get; set; } = null;

    public byte[] PublicKeyDERFormat
    {
        get => _publicKeyDERFormat ??= EncodePublicKeyToAsn1Der();
    }

    private byte[] _publicKeyDERFormat;

    private byte[] EncodePublicKeyToAsn1Der()
    {
        if(RSA is null)
        {
            return null;
        }

        var publicKeyBytes = RSA.ExportRSAPublicKey();

        var writer = new AsnWriter(AsnEncodingRules.DER);
        writer.PushSequence();
        writer.PushSequence();
        writer.WriteObjectIdentifier("1.2.840.113549.1.1.1");
        writer.WriteNull();
        writer.PopSequence();
        writer.WriteBitString(publicKeyBytes);
        writer.PopSequence();

        return writer.Encode();
    }
}
