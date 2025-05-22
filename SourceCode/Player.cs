using System.Collections.Generic;
using Godot;
using NLog;

namespace testMove.SourceCode;

public partial class Player  : Node2D
{
    
     private static readonly ILogger Logger  = LogManager.GetCurrentClassLogger();

    private PlayerAnimationPlayer _animationPlayer;

    private Vector2 _velocity;

    private double _stateSeconds;

    private bool _moving;

    private WeaponType _type;

    private State State { get; set; }
    private Direction Direction { get; set; } 
    
    public int Id { get; private set; }
    

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
    }

    private void ChangeToIdle()
    {
        State = State.Idle;
        _animationPlayer.PlayAnimation(State, Direction);
        Logger.Debug("Current coordinate to {}.", Position.ToCoordinate());
    }
    

    private void WalkTowards(Direction direction)
    {
        Velocities.TryGetValue(direction, out _velocity);
        _velocity /= _animationPlayer.WalkAnimationLength;
        _stateSeconds = 0;
        State = State.Move;
        Direction = direction;
        _animationPlayer.Stop();
        _animationPlayer.PlayAnimation(State.Move, direction);
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
        }
    }

    public void Move(MoveMessage input)
    {
        Position = input.Coordiate.ToPosition();
        Logger.Debug("Set coordinate to {}.", input.Coordiate);
        WalkTowards(input.Direction);
    }

    private void OnAnimationFinished(StringName name)
    {
    }

    public static Player FromMessage(ShowMessage showMessage)
    {
        PackedScene scene = ResourceLoader.Load<PackedScene>("res://Scenes/player.tscn");
        var player = scene.Instantiate<Player>();
        player.Position = showMessage.Coordinate.ToPosition();
        player.ZIndex = 1;
        player.Id = showMessage.Id;
        return player;
    }
}