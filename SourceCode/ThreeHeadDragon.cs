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

    public override void _Ready()
    {
        _audioStreamPlayer = GetNode<AudioStreamPlayer2D>("AudioPlayer");
        _playlist = (AudioStreamPlaylist)_audioStreamPlayer.Stream;
        _state = State.IDLE;
        Logger.Debug("Coordinate {0}.", Position / 32);
    }

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
        }
    }

    public override void _Process(double delta)
    {
        if (_state == State.IDLE)
            OnIdle(delta);
        else if (_state == State.HURT)
            OnGettingHurt(delta);
        else if (_state == State.ATTACKING)
            OnAttacking(delta);
        else if (_state == State.MOVING)
            OnWalking(delta);
    }

    private void OnAttacking(double delta)
    {
        _stateTime += delta;
        if (_stateTime >= 1)
        {
            _state = State.IDLE;
            Play("idle_left2");
        }
    }

    private void OnGettingHurt(double delta)
    {
        _stateTime += delta;
        if (_stateTime >= 1)
        {
            _state = State.IDLE;
            Play("idle_left");
        }
    }
    
    private void OnWalking(double delta)
    {
        _stateTime += delta;
        Position += new Vector2(-32, 0) * (float)delta;
        if (_stateTime >= 1)
        {
            _state = State.IDLE;
            Play("idle_left2");
            Position = Position.Snapped(new Vector2(32, 32));
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is not InputEventKey eventKey )
            return ;
        if (_state != State.IDLE)
            return;
        if (eventKey.Keycode == Key.A)
        {
            _state = State.HURT;
            Play("hit_left");
            _audioStreamPlayer.Stream = _playlist.Stream1;
            _audioStreamPlayer.Play();
            _stateTime = 0;
        }
        else if (eventKey.Keycode == Key.D)
        {
            _state = State.ATTACKING;
            Play("attack_left");
            _audioStreamPlayer.Stream = _playlist.Stream2;
            _audioStreamPlayer.Play();
            _stateTime = 0;
        }
        else if (eventKey.Keycode == Key.K)
        {
            _state = State.DYING;
            Play("die_left");
            _audioStreamPlayer.Stream = _playlist.Stream3;
            _audioStreamPlayer.Play();
        }
        else if (eventKey.Keycode == Key.I)
        {
            _state = State.MOVING;
            Play("walk_left");
            _stateTime = 0;
        }
    }
}