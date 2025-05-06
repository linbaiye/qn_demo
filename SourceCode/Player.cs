using System;
using System.Collections.Generic;
using System.Net;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
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

    private WeaponType _type;

    private State State { get; set; }
    private Direction Direction { get; set; } 
    
    private readonly Bootstrap _bootstrap = new();
    
    private volatile IChannel _channel;


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
        _type = WeaponType.Sword;
        _animationPlayer.SetSwordAnimation();
        _animationPlayer.PlayAnimation(State, Direction);
        _moving = false;
        _animationPlayer.AnimationFinished += OnAnimationFinished;
        SetupNetwork();
    }

    private async void SetupNetwork()
    {
        _bootstrap.Group(new SingleThreadEventLoop()).Handler(
            new ActionChannelInitializer<ISocketChannel>(c => c.Pipeline.AddLast(
                new LengthFieldPrepender(4), 
                new MessageEncoder())
            )).Channel<TcpSocketChannel>();
        _channel = await _bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999));
        await _channel.WriteAndFlushAsync(new MoveMessage(){X =1, Y = 2, Direction = Direction.Down});
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
            if (_moving)
                return;
            if (button.IsPressed())
            {
                _moving = true;
                MoveByMouse();
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
            if (key.Pressed != true)
                return;
            if (key.Keycode == Key.A)
            {
                if (_type == WeaponType.Sword)
                    _animationPlayer.PlayAnimation(PlayerAction.SwordAttack, Direction);
                else if (_type == WeaponType.Axe)
                    _animationPlayer.PlayAnimation(PlayerAction.Axe, Direction);
            }
            else if (key.Keycode == Key.H)
            {
                _animationPlayer.SetHatAnimation();
            }
            else if (key.Keycode == Key.K)
            {
                _animationPlayer.HideHatAnimation();
            }
            else if (key.Keycode == Key.C)
            {
                if (_type == WeaponType.Sword)
                {
                    _type = WeaponType.Axe;
                    _animationPlayer.SetAxeAnimation();
                }
                else if (_type == WeaponType.Axe)
                {
                    _type = WeaponType.Sword;
                    _animationPlayer.SetSwordAnimation();
                }
            }
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
            if (Input.IsMouseButtonPressed(MouseButton.Right))
            {
                MoveByMouse();
            }
            else
            {
                ChangeToIdle();
                _moving = false;
            }
        }
    }
}