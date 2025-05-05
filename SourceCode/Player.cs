using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using NLog;

namespace testMove.SourceCode;

public partial class Player : Node2D
{
    private static readonly ILogger Logger  = LogManager.GetCurrentClassLogger();

    private PlayerAnimationPlayer _animationPlayer;

    private Vector2 _velocity;

    private double _stateSeconds;

    private bool _moving;

    private State State { get; set; }
    private Direction Direction { get; set; } 

    private static readonly IDictionary<Direction, Vector2> Velocities =
        new Godot.Collections.Dictionary<Direction, Vector2>()
        {
            { Direction.Up, new Vector2(0, -32) },
            { Direction.UpRight, new Vector2(32, -32) },
            { Direction.Right, new Vector2(32, 0) },
            { Direction.DownRight, new Vector2(32, 32) },
            { Direction.Down, new Vector2(0, 32) },
            { Direction.DownLeft, new Vector2(-32, 32) },
            { Direction.Left, new Vector2(-32, 0) },
            { Direction.UpLeft, new Vector2(-32, -32) },
        };
        
    
    public override void _Ready()
    {
        _animationPlayer = GetNode<PlayerAnimationPlayer>("AnimationPlayer");
        _animationPlayer.InitializeAnimations();
        State = State.Idle;
        Direction = Direction.Down;
        _animationPlayer.PlayAnimation(State, Direction);
        _moving = false;
        _animationPlayer.AnimationFinished += OnAnimationFinished;
        _animationPlayer.SetSwordAnimation();
    }

    private void ChangeToIdle()
    {
        State = State.Idle;
        _animationPlayer.PlayAnimation(State, Direction);
    }
    
    private Vector2 ParseLine(string s)
    {
        if (!s.Contains(','))
        {
            return new Vector2(0, 0);
        }
        var nobrackets = s.Replace("[", "").Replace("]", "");
        var numbers = nobrackets.Split(",");
        return numbers.Length == 2 ? 
            new Vector2(int.Parse(numbers[0].Trim()), int.Parse(numbers[1].Trim())) :
            new Vector2(0, 0);
    }
    
    private void HandleMouseEvent(InputEventMouse eventMouse)
    {
        if (eventMouse is InputEventMouseButton button && button.ButtonIndex == MouseButton.Right)
        {
            if (button.IsPressed())
            {
                if (_moving)
                    return;
                _moving = true;
                MoveByMouse();
            }
            else
            {
                _moving = false;
            }
        }
    }

    private void MoveByMouse()
    {
        var pos = GetLocalMousePosition();
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
            6 => Direction.Up,
            7 => Direction.UpRight,
            _ => Direction.Right,
        };
        WalkTowards(direction);
    }

    private void WalkTowards(Direction direction)
    {
        Velocities.TryGetValue(direction, out _velocity);
        _velocity /= _animationPlayer.WalkAnimationLength;
        _stateSeconds = 0;
        State = State.Walk;
        Direction = direction;
        _animationPlayer.Stop();
        _animationPlayer.PlayAnimation(State.Walk, direction);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouse mouse)
        {
            HandleMouseEvent(mouse);
        }
        else if (@event is InputEventKey key)
        {
            if (key.Keycode == Key.A)
            {
                _animationPlayer.PlayAnimation(State.Sword, Direction);
            }
            else if (key.Keycode == Key.D)
                _animationPlayer.PlayAnimation(State.SwordHard, Direction);
        }
    }

    private void OnAnimationFinished(StringName name)
    {
        if (!name.ToString().Contains("Walk"))
            ChangeToIdle();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (State == State.Idle)
            return;
        _stateSeconds += delta;
        Position += _velocity * (float)delta;
        if (_stateSeconds >= _animationPlayer.WalkAnimationLength)
        {
            Position = Position.Snapped(new Vector2(32, 32));
            if (_moving)
            {
                MoveByMouse();
            }
            else
            {
                ChangeToIdle();
            }
        }
    }
}