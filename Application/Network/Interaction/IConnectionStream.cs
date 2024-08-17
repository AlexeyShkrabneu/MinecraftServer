﻿namespace Application.Network.Interaction;

public interface IConnectionStream
{
    bool DataAvailable { get; }

    Task<byte> ReadByteAsync(CancellationToken cancellationToken = default);
    Task<byte[]> ReadBytesAsync(int length, CancellationToken cancellationToken = default);
    Task<int> ReadVarIntAsync(CancellationToken cancellationToken = default);
    Task<string> ReadStringAsync(CancellationToken cancellationToken = default);
    Task<ushort> ReadUShortAsync(CancellationToken cancellationToken = default);
    Task<long> ReadLongAsync(CancellationToken cancellationToken = default);

    IConnectionStream WriteVarInt(int value);
    IConnectionStream WriteLong(long value);
    IConnectionStream WriteString(string value);
    IConnectionStream WriteBytes(byte[] value);

    Task SendAsync(CancellationToken cancellationToken = default);
}