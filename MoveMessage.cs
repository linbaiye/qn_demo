
using System;

namespace testMove;

public class MoveMessage : AbstractMessage
{
    public override MessageType MessageType => MessageType.Move;
    public override byte[] ToBytes()
    {
        byte[] bytes = new byte[16];
        byte[] tmp = GetBytes((int)MessageType);
        Array.Copy(tmp, bytes, 4);
        tmp = GetBytes(X);
        Array.Copy(tmp, 0, bytes, 4, 4);
        tmp = GetBytes(Y);
        Array.Copy(tmp, 0, bytes, 8, 4);
        tmp = GetBytes((int)Direction);
        Array.Copy(tmp, 0, bytes, 12, 4);
        return bytes;
    }

    public int X { get; set; }
    public int Y { get; set; }
    public Direction Direction { get; set; }
}