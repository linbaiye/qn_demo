using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using NLog;

namespace testMove.SourceCode;

public partial class Player  : Node2D
{
    
    private static readonly ILogger Logger  = LogManager.GetCurrentClassLogger();
    protected PlayerAnimationPlayer AnimationPlayer { get; private set; }

    private Vector2 _velocity;

    private float _stateElapsedSeconds;
    private float _stateSeconds;

    private WeaponType _weaponType;
    protected State State { get; set; }
    protected Direction Direction { get; set; } 
    public int Id { get; private set; }

    private Queue<MoveMessage> _moveMessages = new ();
    protected event Action<State> StateFinished;

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
        AnimationPlayer = GetNode<PlayerAnimationPlayer>("AnimationPlayer");
        AnimationPlayer.InitializeAnimations();
        Visible = false;
    }

    protected void ChangeToIdle()
    {
        State = State.Idle;
        AnimationPlayer.PlayIdleAnimation(Direction);
        // Logger.Debug("Current coordinate to {}.", Position.ToCoordinate());
    }



    protected void MoveTowards(Direction direction, MoveAction moveAction = MoveAction.Walk)
    {
        _stateSeconds = moveAction == MoveAction.Walk ? AnimationPlayer.WalkAnimationLength : AnimationPlayer.RunAnimationLength;
        Logger.Debug("Movement duration {}.", _stateSeconds);
        Velocities.TryGetValue(direction, out _velocity);
        _velocity /= _stateSeconds;
        _stateElapsedSeconds = 0;
        State = State.Move;
        Direction = direction;
        AnimationPlayer.Stop();
        if (moveAction == MoveAction.Walk)
            AnimationPlayer.PlayWalkAnimation(direction);
        else
        {
            AnimationPlayer.PlayRunAnimation(direction);
        }
    }


    public override void _PhysicsProcess(double delta)
    {
        if (State == State.Idle || _stateElapsedSeconds >= _stateSeconds)
            return;
        _stateElapsedSeconds += (float)delta;
        Position += _velocity * (float)delta;
        if (_stateElapsedSeconds >= _stateSeconds)
        {
            Position = Position.Snapped(new Vector2(32, 32));
            StateFinished?.Invoke(State);
        }
    }

    public void Move(MoveMessage message)
    {
        if (State == State.Move)
        {
            _moveMessages.Enqueue(message);
        }
        else
        {
            Position = message.Coordiate.ToPosition();
            MoveTowards(message.Direction, message.Action);
        }
    }

    public void SetPosition(PositionMessage message)
    {
        Position = message.Coordiate.ToPosition();
        Direction = message.Direction;
        ChangeToIdle();
    }


    public void Equip(WeaponType weaponType)
    {
        _weaponType = weaponType;
        if (_weaponType == WeaponType.Sword) 
            AnimationPlayer.SetSwordAnimation();
        else if (_weaponType == WeaponType.Axe)
            AnimationPlayer.SetAxeAnimation();
    }

    public void Attack(AttackMessage attackMessage)
    {
        Direction = attackMessage.Direction;
        if (_weaponType == WeaponType.Axe)
            AnimationPlayer.PlayAnimation(PlayerAttackAction.Axe, Direction);
        else if (_weaponType == WeaponType.Sword)
            AnimationPlayer.PlayAnimation(PlayerAttackAction.Sword, Direction);
    }

    public void Init(int id, Vector2 coordinate)
    {
        Position = coordinate.ToPosition();
        Id = id;
        State = State.Idle;
        Direction = Direction.Down;
        ZIndex = 1;
        AnimationPlayer.PlayIdleAnimation(Direction);
        Visible = true;
    }

    private void OnStateFinished(State finished)
    {
        if (finished == State.Move)
        {
            MoveMessage? message = null;
            while (_moveMessages.Any())
            {
                message = _moveMessages.Dequeue();
            }
            if (message != null)
            {
                Position = message.Coordiate.ToPosition();
                MoveTowards(message.Direction, message.Action);
            }
            else
            {
                ChangeToIdle();
            }
        }
    }

    private static Player Create(int id, Vector2 coordinate)
    {
        PackedScene scene = ResourceLoader.Load<PackedScene>("res://Scenes/player.tscn");
        var player = scene.Instantiate<Player>();
        player.StateFinished += player.OnStateFinished;
        return player;
    }

    public static Player FromMessage(ShowMessage showMessage)
    {
        return Create(showMessage.Id, showMessage.Coordinate);
    }
}