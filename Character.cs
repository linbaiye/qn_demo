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

    private void HandleMouseEvent(InputEventMouse eventMouse)
    {
        if (eventMouse is not InputEventMouseButton button || button.ButtonMask != MouseButtonMask.Right)
            return;
        var pos= GetLocalMousePosition();
        var angle = Mathf.Snapped(pos.Angle(), Mathf.Pi / 4) / (Mathf.Pi / 4);
        int dir = Mathf.Wrap((int)angle, 0, 8);
        var direction = dir switch
        {
            0 => Direction.Right,
            1 => Direction.DownRight,
            2 => Direction.Down,
            3 => Direction.DownLeft,
            4 => Direction.Left,
            5 => Direction.UpLeft,
            6 => Direction.UpLeft,
            7 => Direction.UpRight,
            _ => Direction.Right,
        };
        Animation animation = new Animation();
        var idx = animation.AddTrack(Animation.TrackType.Value);
        animation.TrackSetPath(idx, ".:offset");
        animation.SetStep(0.6f);
        animation.TrackSetKeyValue(idx, 0, new Vector2(-9, -28));
        animation.TrackSetKeyValue(idx, 1, new Vector2(-9, -28));
        animation.TrackSetKeyValue(idx, 2, new Vector2(-9, -28));
        animation.Length = 1.8f;
        var textureIdx = animation.AddTrack(Animation.TrackType.Value);
        animation.TrackSetPath(textureIdx, ".:texture");
        animation.SetStep(0.6f);
        animation.TrackSetKeyValue(idx, 0, ResourceLoader.Load<Texture2D>("res://char/N02/000054.png"));
        animation.TrackSetKeyValue(idx, 1, ResourceLoader.Load<Texture2D>("res://char/N02/000055.png"));
        animation.TrackSetKeyValue(idx, 2, ResourceLoader.Load<Texture2D>("res://char/N02/000056.png"));
        
        AnimationLibrary animationLibrary = new AnimationLibrary();
        animationLibrary.AddAnimation("right", animation);
        _player.AddAnimationLibrary("idle", animationLibrary);
        Logger.Debug("Direction {0}", (Direction) direction);
    }

    private void InitAnimation()
    {
        
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouse eventMouse)
        {
            HandleMouseEvent(eventMouse);
            return;
        }
        if (@event is not InputEventKey inputEvent)
            return;
        if (_state != State.Idle)
            return;
        if (inputEvent.Keycode == Key.Right)
        {
            _state = State.Walk;
            _stateSeconds = 0;
            var length = _player.GetAnimation("walk_right").Length;
            _velocity = new Vector2(32, 0) / length; 
            _player.Play("walk_right");
        }
        if (inputEvent.Keycode == Key.A)
        {
            _state = State.Attacking;
            _stateSeconds = 0;
            _player.Play("attack_right");
        }
    }

    private void ToIdle()
    {
        _player.Play("idle_right");
        _state = State.Idle;
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
        if (_state == State.Idle)
            return;
        if (_state == State.Attacking)
            OnAttacking(delta);
        else if (_state == State.Walk)
            OnMoving(delta);
    }
}
