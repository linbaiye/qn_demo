using System;

namespace testMove;

public class EquipInput  : AbstractMessage
{
    
    public override MessageType MessageType => MessageType.Equip;

    private WeaponType _weaponType;

    public EquipInput(WeaponType weaponType)
    {
        _weaponType = weaponType;
    }

    public override byte[] ToBytes()
    {
        byte[] bytes = new byte[8];
        byte[] tmp = GetBytes((int)MessageType);
        Array.Copy(tmp, bytes, 4);
        tmp = GetBytes((int)_weaponType);
        Array.Copy(tmp, 0, bytes, 4, tmp.Length);
        return bytes;
    }
    
}