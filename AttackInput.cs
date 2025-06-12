using System;

namespace testMove;

public class AttackInput : AbstractMessage
{

    private readonly Direction _direction;

    public AttackInput(Direction direction)
    {
        _direction = direction;
    }

    public override MessageType MessageType => MessageType.Attack;

    public override byte[] ToBytes()
    {
        byte[] bytes = new byte[8];
        byte[] tmp = GetBytes((int)MessageType);
        Array.Copy(tmp, bytes, 4);
        tmp = GetBytes((int)_direction);
        Array.Copy(tmp, 0, bytes, 4, 4);
        return bytes;
    }
}