namespace GunRitualExorcistEdition.Scripts.Core;

public static class InputManager
{
    public static InputContext CurrentContext { get; private set; }
    
    public static void SetContext<T>(T context) where T : class => CurrentContext = (InputContext)context;
    public static T GetContext<T>() where T : class => CurrentContext as T;
}