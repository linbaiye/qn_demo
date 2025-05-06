using System;
using System.Text;
using System.Text.Json;

namespace testMove;

public abstract class AbstractMessage: IMessage
{
    public abstract MessageType MessageType { get; }

    public abstract byte[] ToBytes();
    
    protected static byte[] GetBytes(int n)
    {
        byte[] bytes = BitConverter.GetBytes(n);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return bytes;
    }
}