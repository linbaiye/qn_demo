using System.Text.Json;
using Godot;

namespace testMove;

public class LoginOkMessage(Vector2 coordinate, int id) 
{
    public int Id => id;

    public Vector2 Coordinate => coordinate;

    public MessageType Type => MessageType.LoginOk;
    
    public static LoginOkMessage Create(int id, int x, int y)
    {
        return new LoginOkMessage(new Vector2(x, y), id);
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}