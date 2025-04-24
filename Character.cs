using Godot;
using System;

public partial class Character : Sprite2D
{
    private bool _moving = false;
    
    private readonly Vector2 VELOCITY = new(32, 0);

    private double _movedSeconds;
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey inputEvent && inputEvent.Keycode == Key.Right)
        {
            _moving = true;
            _movedSeconds = 0;
        }
    }

    public override void _Process(double delta)
    {
        if (!_moving)
            return;
        Position += VELOCITY * (float)delta;
        _movedSeconds += delta;
        if (_movedSeconds >= 1)
            _moving = false;
    }
}
