#nullable enable
using Godot;
using NLog;

namespace testMove.SourceCode;

public partial class Character : Player
{
    private static readonly ILogger Logger  = LogManager.GetCurrentClassLogger();

    private bool _movingPressed;

    private Connection? _connection;

    private bool _footKungFuEnabled;
    
    // private void HandleMouseEvent(InputEventMouse eventMouse)
    // {
    //     if (eventMouse is InputEventMouseButton button && button.ButtonIndex == MouseButton.Right)
    //     {
    //         if (_movingPressed)
    //             return;
    //         if (button.IsPressed())
    //         {
    //             _movingPressed = true;
    //             MoveByMouse();
    //         }
    //     }
    // }

    public void SetFootKungFu(bool enable)
    {
        _footKungFuEnabled = enable;
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
        MoveTowards(direction, _footKungFuEnabled ? MoveAction.Run : MoveAction.Walk);
    }

    private void StopMove()
    {
        _movingPressed = false;
    }
    

    public override void _UnhandledInput(InputEvent @event)
    {

        if (@event is InputEventMouseButton button)
        {
            if (button.ButtonIndex == MouseButton.Right && button.Pressed)
            {
                if (State == State.Move)
                    return;
                MoveByMouse();
                _connection?.WriteAndFlush(MoveInput.Create(Position.ToCoordinate(), Direction));
                _movingPressed = true;
                //Logger.Debug("From {} to {}.", Position.ToCoordinate(), (Position + v).ToCoordinate() );
            }
            else if (button.ButtonIndex == MouseButton.Right && !button.Pressed)
            {
                _movingPressed = false;
            }
        }
        else if (@event is InputEventKey key)
        {
             if (key.Pressed != true)
                 return;
             if (key.Keycode == Key.F)
             {
                 _connection?.WriteAndFlush(new FootKungFuInput());
             }
             else if (key.Keycode == Key.S)
                 _connection?.WriteAndFlush(new EquipInput(WeaponType.Sword));
             else if (key.Keycode == Key.A)
                 _connection?.WriteAndFlush(new EquipInput(WeaponType.Axe));
            // if (key.Keycode == Key.A)
            // {
            //     if (_type == WeaponType.Sword)
            //         AnimationPlayer.PlayAnimation(PlayerAction.SwordAttack, Direction);
            //     else if (_type == WeaponType.Axe)
            //         AnimationPlayer.PlayAnimation(PlayerAction.Axe, Direction);
            // }
            // else if (key.Keycode == Key.H)
            // {
            //     AnimationPlayer.SetHatAnimation();
            // }
            // else if (key.Keycode == Key.K)
            // {
            //     AnimationPlayer.HideHatAnimation();
            // }
            // else if (key.Keycode == Key.C)
            // {
            //     if (_type == WeaponType.Sword)
            //     {
            //         _type = WeaponType.Axe;
            //         AnimationPlayer.SetAxeAnimation();
            //     }
            //     else if (_type == WeaponType.Axe)
            //     {
            //         _type = WeaponType.Sword;
            //         AnimationPlayer.SetSwordAnimation();
            //     }
            // }
        }
    }

    private void OnAnimationFinished(StringName name)
    {
        // if (!name.ToString().Contains("Walk"))
            // ChangeToIdle();
    }

    // public override void _PhysicsProcess(double delta)
    // {
    //     if (State == State.Idle)
    //         return;
    //     _stateSeconds += delta;
    //     Position += _velocity * (float)delta;
    //     if (_stateSeconds >= _animationPlayer.RunAnimationLength)
    //     {
    //         Position = Position.Snapped(new Vector2(32, 32));
    //         if (_movingPressed)
    //         {
    //             MoveByMouse();
    //             _connection?.WriteAndFlush(MoveInput.Create(Position.ToCoordinate(), Direction));
    //             Velocities.TryGetValue(Direction, out Vector2 v);
    //             Logger.Debug("From {} to {}.", Position.ToCoordinate(), (Position + v).ToCoordinate() );
    //         }
    //         else
    //         {
    //             //ChangeToIdle();
    //         }
    //     }
    // }


    private void OnStateFinished(State finishedState)
    {
        if (finishedState == State.Move && _movingPressed)
        {
            MoveByMouse();
            _connection?.WriteAndFlush(MoveInput.Create(Position.ToCoordinate(), Direction));
        }
        else if (!_movingPressed)
        {
            ChangeToIdle();
        }
    }
    
    
    public static Character FromMessage(LoginOkMessage showMessage, Connection connection)
    {
        PackedScene scene = ResourceLoader.Load<PackedScene>("res://Scenes/Character.tscn");
        var character = scene.Instantiate<Character>();
        character.StateFinished += character.OnStateFinished;
        character._connection = connection;
        return character;
    }
}