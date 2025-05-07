using Godot;

namespace testMove;

public static class MethodExtensions
{

    public static Vector2 ToPosition(this Vector2 vector2)
    {
        return vector2 * 32;
    }
    
    public static Vector2 ToCoordinate(this Vector2 vector2)
    {
        return vector2 / 32;
    }
}