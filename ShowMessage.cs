using System.Text.Json;
using Godot;

namespace testMove;

public class ShowMessage(Vector2 coordinate, int id) : IMessage
{
    public int Id => id;

    public Vector2 Coordinate => coordinate;

    public static ShowMessage Create(int id, int x, int y)
    {
        return new ShowMessage(new Vector2(x, y), id);
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }

    public byte[] ToBytes()
    {
        throw new System.NotImplementedException();
    }
}