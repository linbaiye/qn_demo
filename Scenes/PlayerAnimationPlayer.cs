using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using testMove;

public partial class PlayerAnimationPlayer : AnimationPlayer
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private static readonly float WalkTick = 0.14f;
    private static readonly int WalkSpriteNumber = 6;
    
    private AnimationLibrary CreateAnimationLibrary(int spritesPerDirection,
        float step,
        Vector2[] offsets,
        Texture2D[] sprite2Ds,
        Animation.LoopModeEnum loopModeEnum = Animation.LoopModeEnum.None)
    {
        int start = 0;
        AnimationLibrary animationLibrary = new AnimationLibrary();
        var empty = new Texture();
        foreach (var dir in Enum.GetValues(typeof(Direction)))
        {
            Animation animation = new Animation();
            animation.Length = step * spritesPerDirection;
            animation.LoopMode = loopModeEnum;
            var textureIdx = animation.AddTrack(Animation.TrackType.Value);
            var offsetIdx = animation.AddTrack(Animation.TrackType.Value);
            var weaponTextureIdx = animation.AddTrack(Animation.TrackType.Value);
            var weaponOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
            var hatTextureIdx = animation.AddTrack(Animation.TrackType.Value);
            var hatOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
            animation.TrackSetPath(textureIdx, "Body:texture");
            animation.TrackSetPath(offsetIdx, "Body:offset");
            animation.TrackSetPath(weaponTextureIdx, "Weapon:texture");
            animation.TrackSetPath(weaponOffsetIdx, "Weapon:offset");
            animation.TrackSetPath(hatTextureIdx, "Hat:texture");
            animation.TrackSetPath(hatOffsetIdx, "Hat:offset");
            animation.ValueTrackSetUpdateMode(offsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(offsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(weaponOffsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(weaponTextureIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(hatOffsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(hatTextureIdx, Animation.UpdateMode.Discrete);
            for (int i = 0; i < spritesPerDirection; i++)
            {
                animation.TrackInsertKey(weaponOffsetIdx, step * i, Vector2.Zero);
                animation.TrackInsertKey(weaponTextureIdx, step * i, empty);
                animation.TrackInsertKey(hatOffsetIdx, step * i, Vector2.Zero);
                animation.TrackInsertKey(hatTextureIdx, step * i, empty);
                animation.TrackInsertKey(offsetIdx, step * i, offsets[start + i]);
                animation.TrackInsertKey(textureIdx, step * i, sprite2Ds[start + i]);
            }
            animationLibrary.AddAnimation(dir.ToString(), animation);
            start += spritesPerDirection;
        }
        return animationLibrary;
    }

    private AnimationLibrary CreateAnimationLibrary(int spriteStart, int spritesPerDirection, float step,
        Vector2[] offsets, Animation.LoopModeEnum loopModeEnum = Animation.LoopModeEnum.None, string subdir = "N00")
    {
        int start = spriteStart;
        AnimationLibrary animationLibrary = new AnimationLibrary();
        var empty = new Texture();
        foreach (var dir in Enum.GetValues(typeof(Direction)))
        {
            Animation animation = new Animation();
            animation.Length = step * spritesPerDirection;
            animation.LoopMode = loopModeEnum;
            var textureIdx = animation.AddTrack(Animation.TrackType.Value);
            var offsetIdx = animation.AddTrack(Animation.TrackType.Value);
            var weaponTextureIdx = animation.AddTrack(Animation.TrackType.Value);
            var weaponOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
            var hatTextureIdx = animation.AddTrack(Animation.TrackType.Value);
            var hatOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
            animation.TrackSetPath(textureIdx, "Body:texture");
            animation.TrackSetPath(offsetIdx, "Body:offset");
            animation.TrackSetPath(weaponTextureIdx, "Weapon:texture");
            animation.TrackSetPath(weaponOffsetIdx, "Weapon:offset");
            animation.TrackSetPath(hatTextureIdx, "Hat:texture");
            animation.TrackSetPath(hatOffsetIdx, "Hat:offset");
            animation.ValueTrackSetUpdateMode(offsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(offsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(weaponOffsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(weaponTextureIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(hatOffsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(hatTextureIdx, Animation.UpdateMode.Discrete);
            for (int i = 0; i < spritesPerDirection; i++)
            {
                animation.TrackInsertKey(weaponOffsetIdx, step * i, Vector2.Zero);
                animation.TrackInsertKey(weaponTextureIdx, step * i, empty);
                animation.TrackInsertKey(hatOffsetIdx, step * i, Vector2.Zero);
                animation.TrackInsertKey(hatTextureIdx, step * i, empty);
                animation.TrackInsertKey(offsetIdx, step * i, offsets[start + i]);
                var spriteIndex = start + i;
                animation.TrackInsertKey(textureIdx, step * i,
                    ResourceLoader.Load<Texture2D>($"res://char/{subdir}/{spriteIndex:D6}.png"));
            }
            animationLibrary.AddAnimation(dir.ToString(), animation);
            start += spritesPerDirection;
        }
        return animationLibrary;
    }

    private Vector2 ParseLine(string s)
    {
        if (!s.Contains(','))
        {
            return new Vector2(0, 0);
        }

        var nobrackets = s.Replace("[", "").Replace("]", "");
        var numbers = nobrackets.Split(",");
        return numbers.Length == 2
            ? new Vector2(int.Parse(numbers[0].Trim()), int.Parse(numbers[1].Trim()))
            : new Vector2(0, 0);
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

    private AnimationLibrary CreateWalkAnimations(Vector2[] offsets)
    {
        return CreateAnimationLibrary(0, WalkSpriteNumber, WalkTick, offsets);
    }
    
    private AnimationLibrary CreateWalkAnimations(Vector2[] offsets, Texture2D[] texture2Ds)
    {
        return CreateAnimationLibrary(WalkSpriteNumber,  WalkTick, offsets, texture2Ds);
    }
    
    
    private AnimationLibrary CreateStopWalkAnimations(Vector2[] offsets, Texture2D[] texture2Ds)
    {
        Vector2[] off = new Vector2[16];
        Texture2D[] tex  = new Texture2D[16];
        int walk = 0;
        int stop = 48;
        int index = 0;
        for (int i = 0; i < 8; i++)
        {
            off[index] = offsets[walk];
            tex[index++] = texture2Ds[walk];
            // off[index] = offsets[stop];
            // tex[index++] = texture2Ds[stop];
            walk += 6;
            stop += 3;
        }
        return CreateAnimationLibrary(1,  WalkTick, off, tex);
    }
    
    private AnimationLibrary CreateIdleAnimations(Vector2[] offsets)
    {
        return CreateAnimationLibrary(48, 3, 0.1f, offsets, Animation.LoopModeEnum.Linear);
    }

    private AnimationLibrary CreateSwordAttackAnimations(Vector2[] offsets)
    {
        return CreateAnimationLibrary(0, 9, 0.07f, offsets, Animation.LoopModeEnum.None, "N02");
    }
    
    private AnimationLibrary CreateAxeAnimations(Vector2[] offsets)
    {
        return CreateAnimationLibrary(0, 8, 0.1f, offsets, Animation.LoopModeEnum.None, "N03");
    }
    
    
    private AnimationLibrary CreateSwordHardAttackAnimations(Vector2[] offsets)
    {
        return CreateAnimationLibrary(144, 10, 0.1f, offsets, Animation.LoopModeEnum.None, "N02");
    }

    public float WalkAnimationLength => WalkSpriteNumber * WalkTick;
    public float StopWalkLength =>  WalkTick;

    private static readonly float RunSpeed = 1.4f;
    public float RunAnimationLength => WalkAnimationLength / RunSpeed;


    private Texture2D[] LoadTextures(string subdir, int number)
    {
        Texture2D[] sprite2Ds = new Texture2D[number];
        for (int i = 0; i < number; i++)
        {
            sprite2Ds[i] = ResourceLoader.Load<Texture2D>($"res://char/{subdir}/{i:D6}.png");
        }
        return sprite2Ds;
    }

    public void InitializeAnimations()
    {
        var n02Offsets = LoadOffsets("N00");
        var n020Textures = LoadTextures("N00", n02Offsets.Length);
        AddAnimationLibrary(State.Move.ToString(), CreateWalkAnimations(n02Offsets, n020Textures));
        AddAnimationLibrary(State.StopMove.ToString(), CreateStopWalkAnimations(n02Offsets, n020Textures));
        AddAnimationLibrary(State.Idle.ToString(), CreateIdleAnimations(n02Offsets));
        var n00Offsets = LoadOffsets("N02");
        AddAnimationLibrary(PlayerAttackAction.Sword.ToString(), CreateSwordAttackAnimations(n00Offsets));
        AddAnimationLibrary(PlayerAttackAction.Sword2H.ToString(), CreateSwordHardAttackAnimations(n00Offsets));
        var n003ffsets = LoadOffsets("N03");
        AddAnimationLibrary(PlayerAttackAction.Axe.ToString(), CreateAxeAnimations(n003ffsets));
    }

    public void SetAxeAnimation()
    {
        var offsets = LoadOffsets("w130");
        Dictionary<State, int> stateSpriteStart = new Dictionary<State, int>()
        {
            { State.Move, 0 },
            { State.Idle, 48 },
        };
        Dictionary<PlayerAttackAction, int> attackAction = new Dictionary<PlayerAttackAction, int>()
        {
            { PlayerAttackAction.Axe, 0 },
        };
        foreach (var state in stateSpriteStart.Keys)
        {
            stateSpriteStart.TryGetValue(state, out var spriteIndex);
            foreach (var dir in Enum.GetValues(typeof(Direction)))
            {
                var animation = GetAnimation(state+ "/" + dir);
                int count = animation.TrackGetKeyCount(2);
                for (int i = 0; i < count; i++)
                {
                    animation.TrackSetKeyValue(2, i,
                        ResourceLoader.Load<Texture2D>($"res://char/w130/{spriteIndex:D6}.png"));
                    animation.TrackSetKeyValue(3, i, offsets[spriteIndex]);
                    spriteIndex++;
                }
            }
        }
        offsets = LoadOffsets("w133");
        foreach (var action in attackAction.Keys)
        {
            attackAction.TryGetValue(action, out var spriteIndex);
            foreach (var dir in Enum.GetValues(typeof(Direction)))
            {
                var animation = GetAnimation(action+ "/" + dir);
                int count = animation.TrackGetKeyCount(2);
                for (int i = 0; i < count; i++)
                {
                    animation.TrackSetKeyValue(2, i,
                        ResourceLoader.Load<Texture2D>($"res://char/w133/{spriteIndex:D6}.png"));
                    animation.TrackSetKeyValue(3, i, offsets[spriteIndex]);
                    spriteIndex++;
                }
            }
        }
    }

    public void HideHatAnimation()
    {
        State[] states = [State.Idle, State.Move];
        Texture empty = new Texture();
        foreach (var state in states)
        {
            foreach (var dir in Enum.GetValues(typeof(Direction)))
            {
                var animation = GetAnimation(state+ "/" + dir);
                int count = animation.TrackGetKeyCount(0);
                for (int i = 0; i < count; i++)
                {
                    animation.TrackSetKeyValue(4, i, empty);
                    animation.TrackSetKeyValue(5, i, Vector2.Zero);
                }
            }
        }
    }

    public void SetHatAnimation()
    {
        var offsets = LoadOffsets("v160");
        Dictionary<State, int> stateSpriteStart = new Dictionary<State, int>()
        {
            { State.Move, 0 },
            { State.Idle, 48 },
        };
        
        foreach (var state in stateSpriteStart.Keys)
        {
            stateSpriteStart.TryGetValue(state, out var spriteIndex);
            foreach (var dir in Enum.GetValues(typeof(Direction)))
            {
                var animation = GetAnimation(state+ "/" + dir);
                int count = animation.TrackGetKeyCount(0);
                for (int i = 0; i < count; i++)
                {
                    animation.TrackSetKeyValue(4, i,
                        ResourceLoader.Load<Texture2D>($"res://char/v160/{spriteIndex:D6}.png"));
                    animation.TrackSetKeyValue(5, i, offsets[spriteIndex]);
                    spriteIndex++;
                }
            }
        }
    }
    
    public void SetSwordAnimation()
    {
        var offsets = LoadOffsets("w10");
        Dictionary<string, int> stateSpriteStart = new Dictionary<string, int>()
        {
            { State.Move.ToString(), 0 },
            { State.Idle.ToString(), 48 },
        };
        Dictionary<PlayerAttackAction, int> swordStateSpriteStart = new Dictionary<PlayerAttackAction, int>()
        {
            { PlayerAttackAction.Sword, 0 },
            { PlayerAttackAction.Sword2H, 144 },
        };
        foreach (var state in stateSpriteStart.Keys)
        {
            stateSpriteStart.TryGetValue(state, out var spriteIndex);
            foreach (var dir in Enum.GetValues(typeof(Direction)))
            {
                var animation = GetAnimation(state+ "/" + dir);
                int count = animation.TrackGetKeyCount(2);
                for (int i = 0; i < count; i++)
                {
                    animation.TrackSetKeyValue(2, i,
                        ResourceLoader.Load<Texture2D>($"res://char/w10/{spriteIndex:D6}.png"));
                    animation.TrackSetKeyValue(3, i, offsets[spriteIndex]);
                    spriteIndex++;
                }
            }
        }
        offsets = LoadOffsets("w12");
        foreach (var state in swordStateSpriteStart.Keys)
        {
            swordStateSpriteStart.TryGetValue(state, out var spriteIndex);
            foreach (var dir in Enum.GetValues(typeof(Direction)))
            {
                var animation = GetAnimation(state+ "/" + dir);
                int count = animation.TrackGetKeyCount(2);
                for (int i = 0; i < count; i++)
                {
                    animation.TrackSetKeyValue(2, i,
                        ResourceLoader.Load<Texture2D>($"res://char/w12/{spriteIndex:D6}.png"));
                    animation.TrackSetKeyValue(3, i, offsets[spriteIndex]);
                    spriteIndex++;
                }
            }
        }
    }

    public void PlayIdleAnimation(Direction direction)
    {
        Play(State.Idle + "/" + direction, -1, 0.16666f);
    }
    
    public void PlayFlyAnimation(Direction direction)
    {
        Play(State.Idle + "/" + direction);
    }

    public void PlayWalkAnimation(Direction direction)
    {
        Play(State.Move + "/" + direction);
    }
    

    public void PlayStopWalkAnimation(Direction direction)
    {
        Play(State.StopMove + "/" + direction);
    }
    
    public void PlayRunAnimation(Direction direction)
    {
        Play(State.Move + "/" + direction, -1, RunSpeed);
    }
    
    public void PlayAnimation(State state, Direction direction)
    {
        Play(state + "/" + direction);
    }
    
    public void PlayAnimation(PlayerAttackAction playerAttackAction, Direction direction)
    {
        Play(playerAttackAction + "/" + direction);
    }
}
