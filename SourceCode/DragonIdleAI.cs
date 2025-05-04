using Godot;

namespace testMove.SourceCode;

public class DragonIdleAI
{
    private ThreeHeadDragon? _dragon;
    
    public ThreeHeadDragon Dragon
    {
        get => _dragon;
        set => _dragon = value;
    }

    public void Update(double delta)
    {
    }

    public void Handle(InputEvent inputEvent)
    {
        
    }
}