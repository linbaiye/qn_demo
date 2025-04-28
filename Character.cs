using Godot;
using System;

public partial class Character : AnimatedSprite2D
{
    private bool _moving;

    private Vector2 _velocity = Vector2.Zero;

    private double _movedSeconds;
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is not InputEventKey inputEvent)
            return;
        if (inputEvent.Keycode == Key.Right) {
            _moving = true;
            _movedSeconds = 0;
            _velocity = new Vector2(32, 0);
        }
        else if (inputEvent.Keycode == Key.Down)
        {
            _moving = true;
            _movedSeconds = 0;
            _velocity = new Vector2(0, 32);
        }
    }

    public override void _Process(double delta)
    {
        if (!_moving)
            return;
        Position += _velocity * (float)delta;
        _movedSeconds += delta;
        if (_movedSeconds >= 1)
            _moving = false;
    }
}
