using Godot;

namespace testMove;

public class MoveMessage
{
    public MoveMessage(int id, int x, int y, int direction)
    {
        X = x;
        Y = y;
        Direction = (Direction)direction;
        Id = id;
    }

    public Vector2 Coordiate => new Vector2(X, Y);

    public int X { get; set; }
    public int Y { get; set; }
    
    public Direction Direction { get; set; }
    
    public int Id { get; set; }
    
}