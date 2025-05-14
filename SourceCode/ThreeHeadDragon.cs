using System;
using Godot;
using NLog;

namespace testMove.SourceCode;

public partial class ThreeHeadDragon : AnimatedSprite2D
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    
    private double _elapsedTime = 0;
    private double _stateTime = 0;
    
    private AudioStreamPlayer2D _audioStreamPlayer;
    
    private AudioStreamPlaylist _playlist;

    private State _state;

    private Vector2 _velocity;
    
    public override void _Ready()
    {
        _audioStreamPlayer = GetNode<AudioStreamPlayer2D>("AudioPlayer");
        _playlist = (AudioStreamPlaylist)_audioStreamPlayer.Stream;
        Logger.Debug("Coordinate {0}.", Position / 32);
        ChangeToIdle();
    }

    private void PlayIdleAnimation()
    {
        if (Direction == Direction.Left)
            Play("idle_left");
        else if (Direction == Direction.Right)
            Play("idle_right");
        else if (Direction == Direction.Up)
            Play("idle_up");
        else if (Direction == Direction.Down)
            Play("idle_down");
        else if (Direction == Direction.UpRight)
            Play("idle_up_right");
        else if (Direction == Direction.DownRight)
            Play("idle_down_right");
        else if (Direction == Direction.DownLeft)
            Play("idle_down_left");
        else if (Direction == Direction.UpLeft)
            Play("idle_up_left");
    }

    public Direction Direction { get; set; }


    private void OnIdle(double delta)
    {
        _elapsedTime += delta;
        if (_elapsedTime >= 10)
        {
            if (new Random().NextInt64() % 2 == 1)
            {
                _audioStreamPlayer.Stream = _playlist.Stream0;
                _audioStreamPlayer.Play();
            }
            _elapsedTime = 0;
            Walk();
        }
    }


    private void ChangeToIdle()
    {
        _state = State.Idle;
        Direction = (Direction)new Random().Next((int)Direction.Up, (int)Direction.UpLeft);
        PlayIdleAnimation();
    }
    
    public void Walk()
    {
        _state = State.Move;
        _stateTime = 0;
        if (Direction == Direction.Left)
        {
            Play("walk_left");
            _velocity = new Vector2(-32, 0);
        }
        else if (Direction == Direction.Right)
        {
            Play("walk_right");
            _velocity = new Vector2(32, 0);
        }
        else if (Direction == Direction.Up)
        {
            Play("walk_up");
            _velocity = new Vector2(0, -32);
        }
        else if (Direction == Direction.Down)
        {
            Play("walk_down");
            _velocity = new Vector2(0, 32);
        }
        else if (Direction == Direction.UpRight)
        {
            Play("walk_up_right");
            _velocity = new Vector2(32, -32);
        }
        else if (Direction == Direction.DownRight)
        {
            Play("walk_down_right");
            _velocity = new Vector2(32, 32);
        }
        else if (Direction == Direction.DownLeft)
        {
            Play("walk_down_left");
            _velocity = new Vector2(-32, 32);
        }
        else if (Direction == Direction.UpLeft)
        {
            Play("walk_up_left");
            _velocity = new Vector2(-32, -32);
        }
    }

    public override void _Process(double delta)
    {
        if (_state == State.Idle)
            OnIdle(delta);
        else if (_state == State.Hurt)
            OnGettingHurt(delta);
        else if (_state == State.Attack)
            OnAttacking(delta);
        else if (_state == State.Move)
            OnWalking(delta);
    }

    private void OnAttacking(double delta)
    {
        _stateTime += delta;
        if (_stateTime >= 1)
        {
            _state = State.Idle;
            Play("idle_left2");
        }
    }

    private void OnGettingHurt(double delta)
    {
        _stateTime += delta;
        if (_stateTime >= 1)
        {
            _state = State.Idle;
            Play("idle_left");
        }
    }
    
    private void OnWalking(double delta)
    {
        _stateTime += delta;
        Position += _velocity * (float)delta;
        if (_stateTime >= 1)
        {
            Position = Position.Snapped(new Vector2(32, 32));
            ChangeToIdle();
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // if (@event is not InputEventKey eventKey )
        //     return ;
        // if (_state != State.IDLE)
        //     return;
        // if (eventKey.Keycode == Key.A)
        // {
        //     _state = State.HURT;
        //     Play("hit_left");
        //     _audioStreamPlayer.Stream = _playlist.Stream1;
        //     _audioStreamPlayer.Play();
        //     _stateTime = 0;
        // }
        // else if (eventKey.Keycode == Key.D)
        // {
        //     _state = State.ATTACKING;
        //     Play("attack_left");
        //     _audioStreamPlayer.Stream = _playlist.Stream2;
        //     _audioStreamPlayer.Play();
        //     _stateTime = 0;
        // }
        // else if (eventKey.Keycode == Key.K)
        // {
        //     _state = State.DYING;
        //     Play("die_left");
        //     _audioStreamPlayer.Stream = _playlist.Stream3;
        //     _audioStreamPlayer.Play();
        // }
        // else if (eventKey.Keycode == Key.I)
        // {
        //     _state = State.MOVING;
        //     Play("walk_left");
        //     _stateTime = 0;
        // }
    }
}