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

    private AnimationPlayer _animationPlayer;

    private Vector2 _velocity;

    private double _stateSeconds;

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
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _animationPlayer.AnimationFinished += OnAnimationFinished;
        InitializeAnimations();
        State = State.Idle;
        Direction = Direction.Down;
        PlayAnimation(State, Direction);
    }

    private void ChangeToIdle()
    {
        State = State.Idle;
        PlayAnimation(State, Direction);
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
    
    private Vector2[] ParseVectors(IEnumerable<string> lines)
    {
        return (from line in lines where line.Contains(',') select ParseLine(line)).ToArray();
    }
    
    private static readonly string DIR_PATH = "char/";
    
    private Vector2[] LoadOffsets(string name)
    {
        var spriteDirPath = DIR_PATH + name.ToLower() + "/";
        var offsets = File.ReadLines(spriteDirPath + "offset.txt");
        return ParseVectors(offsets);
    }


    private AnimationLibrary CreateAnimationLibrary(int spriteStart, int spritesPerDirection, float step,
        Vector2[] offsets, Animation.LoopModeEnum loopModeEnum = Animation.LoopModeEnum.None)
    {
        int start = spriteStart;
        AnimationLibrary animationLibrary = new AnimationLibrary();
        foreach (var dir in Enum.GetValues(typeof(Direction)))
        {
            // Logger.Debug("Loading sprites starting from {}.", start);
            Animation animation = new Animation();
            animation.Length = step * spritesPerDirection;
            animation.LoopMode = loopModeEnum;
            var idx = animation.AddTrack(Animation.TrackType.Value);
            var textureIdx = animation.AddTrack(Animation.TrackType.Value);
            animation.TrackSetPath(idx, "Body:offset");
            animation.ValueTrackSetUpdateMode(idx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(textureIdx, Animation.UpdateMode.Discrete);
            animation.TrackSetPath(textureIdx, "Body:texture");
            for (int i = 0; i < spritesPerDirection; i++)
            {
                animation.TrackInsertKey(idx, step * i, offsets[start + i]);
                if ((Direction)dir == Direction.Right)
                {
                    Logger.Debug("Offset {}.", offsets[start + i]);
                    
                }
                var spriteIndex = start + i;
                animation.TrackInsertKey(textureIdx, step * i,
                    ResourceLoader.Load<Texture2D>($"res://char/N02/{spriteIndex:D6}.png"));
            }
            animationLibrary.AddAnimation(dir.ToString(), animation);
            Logger.Debug("Added animation for direction {}, starting from {}.", dir, start);
            start += spritesPerDirection;
        }
        return animationLibrary;
        
    }

    
    private AnimationLibrary CreateWalkAnimations(Vector2[] offsets)
    {
        return CreateAnimationLibrary(0, 6, 0.14f, offsets);
    }

    private AnimationLibrary CreateIdleAnimations(Vector2[] offsets)
    {
        return CreateAnimationLibrary(48, 3, 0.6f, offsets, Animation.LoopModeEnum.Linear);
    }

    private void HandleMouseEvent(InputEventMouse eventMouse)
    {
        if (eventMouse is not InputEventMouseButton button || button.ButtonMask != MouseButtonMask.Right)
            return;
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
        Logger.Debug("Direction {}.", direction);
        Velocities.TryGetValue(direction, out _velocity);
        _velocity /= 0.84f;
        _stateSeconds = 0;
        PlayAnimation(State.Walk, direction);
        WalkTowards(direction);
    }

    private void WalkTowards(Direction direction)
    {
        State = State.Walk;
        Direction = direction;
        PlayAnimation(State.Walk, direction);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouse mouse)
        {
            HandleMouseEvent(mouse);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (State == State.Idle)
            return;
        _stateSeconds += delta;
        Position += _velocity * (float)delta;
        if (_stateSeconds >= 0.84)
        {
            Position = Position.Snapped(new Vector2(32, 32));
            ChangeToIdle();
        }
    }

    private void InitializeAnimations()
    {
        var n02Offsets = LoadOffsets("N02");
        _animationPlayer.AddAnimationLibrary(State.Walk.ToString(), CreateWalkAnimations(n02Offsets));
        _animationPlayer.AddAnimationLibrary(State.Idle.ToString(), CreateIdleAnimations(n02Offsets));
    }

    private void OnAnimationFinished(StringName name)
    {
        Logger.Debug("Animation {} done.", name);
    }

    private void PlayAnimation(State state, Direction direction)
    {
        _animationPlayer.Stop();
        _animationPlayer.Play(state + "/" + direction);
    }
}