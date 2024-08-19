namespace Domain.Models.Encryption;

public class ServerEncryption
{
    public RSA RSA { get; set; } = null;

    public byte[] PublicKey 
    { 
        get => _publicKey ??= RSA?.ExportRSAPublicKey();
    }

    public byte[] PublicKeyDERFormat
    {
        get => _publicKeyDERFormat ??= EncodePublicKeyToAsn1Der();
    }

    private byte[] _publicKey;
    private byte[] _publicKeyDERFormat;

    private byte[] EncodePublicKeyToAsn1Der()
    {
        if(RSA is null)
        {
            return null;
        }

        var writer = new AsnWriter(AsnEncodingRules.DER);
        writer.PushSequence();
        writer.PushSequence();
        writer.WriteObjectIdentifier("1.2.840.113549.1.1.1");
        writer.WriteNull();
        writer.PopSequence();
        writer.WriteBitString(PublicKey);
        writer.PopSequence();

        return writer.Encode();
    }
}
