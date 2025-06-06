namespace testMove;

public class EquipMessage
{

    public EquipMessage(int id, WeaponType weaponType)
    {
        Id = id;
        WeaponType = weaponType;
    }
    
    public int Id { get; }
    public WeaponType WeaponType { get; }
}