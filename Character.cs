using Godot;
using System;
using NLog;
using testMove;

public partial class Character : Sprite2D
{
    private Vector2 _velocity = Vector2.Zero;

    private double _movedSeconds;

    private AnimationPlayer _player;
    
    private static readonly ILogger Logger  = LogManager.GetCurrentClassLogger();
    
    private bool _attacking;

    private double _stateSeconds;

    private State _state;
    
    public override void _Ready()
    {
        _player = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is not InputEventKey inputEvent)
            return;
        if (_state != State.IDLE)
            return;
        if (inputEvent.Keycode == Key.Right)
        {
            _state = State.MOVING;
            _stateSeconds = 0;
            var length = _player.GetAnimation("walk_right").Length;
            _velocity = new Vector2(32, 0) / length; 
            _player.Play("walk_right");
        }
        if (inputEvent.Keycode == Key.A)
        {
            _state = State.ATTACKING;
            _stateSeconds = 0;
            _player.Play("attack_right");
        }
    }

    private void ToIdle()
    {
        _player.Play("idle_right");
        _state = State.IDLE;
        _stateSeconds = 0;
    }

    private void OnAttacking(double delta)
    {
        var length = _player.GetAnimation("attack_right").Length;
        _stateSeconds += delta;
        if (_stateSeconds >= length)
        {
            _player.Stop();
            ToIdle();
        }
    }
    
    private void OnMoving(double delta)
    {
        var length = _player.GetAnimation("walk_right").Length;
        Position += _velocity * (float)delta;
        _stateSeconds += delta;
        if (_stateSeconds >= length)
        {
            Position = Position.Snapped(new Vector2(32, 32));
            _player.Stop();
            ToIdle();
            Logger.Debug("Coordinate {0}.", Position / 32);
        }
    }

    public override void _Process(double delta)
    {
        if (_state == State.IDLE)
            return;
        if (_state == State.ATTACKING)
            OnAttacking(delta);
        else if (_state == State.MOVING)
            OnMoving(delta);
    }
}
