namespace testMove;

public class AttackMessage
{
    public AttackMessage(int id, Direction direction)
    {
        Direction = direction;
        Id = id;
    }

    public int Id { get; }
    
    public Direction Direction { get; }
    
}