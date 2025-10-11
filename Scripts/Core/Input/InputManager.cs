using GunRitualExorcistEdition.Scripts.Core;

public static class InputManager
{
    private static InputContext _currentContext = new ControlContext();
    
    public static void SetContext<T>(T context) where T : class => _currentContext = (InputContext)context;
    public static T GetContext<T>() where T : class => _currentContext as T;
}