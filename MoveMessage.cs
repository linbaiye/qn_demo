using Godot;

namespace testMove;

public class MoveMessage
{
    public MoveMessage(int id, int x, int y, int direction, MoveAction action)
    {
        X = x;
        Y = y;
        Direction = (Direction)direction;
        Id = id;
        Action = action;
    }

    public Vector2 Coordiate => new Vector2(X, Y);
    
    public MoveAction Action { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    
    public Direction Direction { get; set; }
    
    public int Id { get; set; }
    
}