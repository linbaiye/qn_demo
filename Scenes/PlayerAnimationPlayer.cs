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

    private AnimationLibrary CreateAnimationLibrary(int spriteStart, int spritesPerDirection, float step,
        Vector2[] offsets, Animation.LoopModeEnum loopModeEnum = Animation.LoopModeEnum.None, string subdir = "N02")
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
            // Logger.Debug("tid {}, oid {}, wtid {} woid {}.", textureIdx, offsetIdx, weaponTextureIdx, weaponOffsetIdx);
            animation.TrackSetPath(textureIdx, "Body:texture");
            animation.TrackSetPath(offsetIdx, "Body:offset");
            animation.TrackSetPath(weaponTextureIdx, "Weapon:texture");
            animation.TrackSetPath(weaponOffsetIdx, "Weapon:offset");
            animation.ValueTrackSetUpdateMode(offsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(offsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(weaponOffsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(weaponTextureIdx, Animation.UpdateMode.Discrete);
            for (int i = 0; i < spritesPerDirection; i++)
            {
                int key1 = animation.TrackInsertKey(weaponOffsetIdx, step * i, Vector2.Zero);
                int key2 = animation.TrackInsertKey(weaponTextureIdx, step * i, empty);
                Logger.Debug("Key1 {}, kye2 {}.", key1, key2);
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
        return CreateAnimationLibrary(0, 6, 0.14f, offsets);
    }

    private AnimationLibrary CreateIdleAnimations(Vector2[] offsets)
    {
        return CreateAnimationLibrary(48, 3, 0.6f, offsets, Animation.LoopModeEnum.Linear);
    }

    private AnimationLibrary CreateSwordAnimations(Vector2[] offsets)
    {
        return CreateAnimationLibrary(0, 9, 0.07f, offsets, Animation.LoopModeEnum.None, "N00");
    }
    
    private AnimationLibrary CreateAxeAnimations(Vector2[] offsets)
    {
        return CreateAnimationLibrary(0, 8, 0.1f, offsets, Animation.LoopModeEnum.None, "N03");
    }
    
    
    private AnimationLibrary CreateSwordHardAnimations(Vector2[] offsets)
    {
        return CreateAnimationLibrary(144, 10, 0.1f, offsets, Animation.LoopModeEnum.None, "N00");
    }

    public float WalkAnimationLength => 0.84f;

    public void InitializeAnimations()
    {
        var n02Offsets = LoadOffsets("N02");
        AddAnimationLibrary(State.Walk.ToString(), CreateWalkAnimations(n02Offsets));
        AddAnimationLibrary(State.Idle.ToString(), CreateIdleAnimations(n02Offsets));
        var n00Offsets = LoadOffsets("N00");
        AddAnimationLibrary(AttackAction.Sword.ToString(), CreateSwordAnimations(n00Offsets));
        AddAnimationLibrary(AttackAction.Sword2H.ToString(), CreateSwordHardAnimations(n00Offsets));
        var n003ffsets = LoadOffsets("N03");
        AddAnimationLibrary(AttackAction.Axe.ToString(), CreateAxeAnimations(n003ffsets));
    }

    public void SetAxeAnimation()
    {
        var offsets = LoadOffsets("w130");
        Dictionary<State, int> stateSpriteStart = new Dictionary<State, int>()
        {
            { State.Walk, 0 },
            { State.Idle, 48 },
        };
        Dictionary<AttackAction, int> swordStateSpriteStart = new Dictionary<AttackAction, int>()
        {
            { AttackAction.Axe, 0 },
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
                        ResourceLoader.Load<Texture2D>($"res://char/w133/{spriteIndex:D6}.png"));
                    animation.TrackSetKeyValue(3, i, offsets[spriteIndex]);
                    spriteIndex++;
                }
            }
        }
    }
    
    public void SetSwordAnimation()
    {
        var offsets = LoadOffsets("w10");
        Dictionary<State, int> stateSpriteStart = new Dictionary<State, int>()
        {
            { State.Walk, 0 },
            { State.Idle, 48 },
        };
        Dictionary<AttackAction, int> swordStateSpriteStart = new Dictionary<AttackAction, int>()
        {
            { AttackAction.Sword, 0 },
            { AttackAction.Sword2H, 144 },
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
    
    public void PlayAnimation(State state, Direction direction)
    {
        Play(state + "/" + direction);
    }
    
    public void PlayAnimation(AttackAction attackAction, Direction direction)
    {
        Play(attackAction + "/" + direction);
    }
}
