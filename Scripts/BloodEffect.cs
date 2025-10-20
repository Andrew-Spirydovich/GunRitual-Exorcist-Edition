using Godot;

namespace GunRitualExorcistEdition.Scripts;

public partial  class BloodEffect: AnimatedSprite2D
{
    public override void _Ready()
    {
        AnimationFinished += QueueFree;
    }

    public void PlayRandom()
    {
        var randomIndex = GD.RandRange(1, 4);
        var animationName = $"Blood{randomIndex}";
        Animation = animationName;
        Frame = 0;
        Play();
    }
}