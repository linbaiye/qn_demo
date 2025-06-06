using System;

namespace testMove;

public class FootKungFuInput : AbstractMessage
{

    public override MessageType MessageType => MessageType.FootKungFu;
    public override byte[] ToBytes()
    {
        byte[] bytes = new byte[4];
        byte[] tmp = GetBytes((int)MessageType);
        Array.Copy(tmp, bytes, 4);
        return bytes;
    }
}