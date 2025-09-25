namespace GunRitualExorcistEdition.Scripts.Core;

public static class InputManager
{
    public static InputContext CurrentContext { get; private set; } = InputContext.Gameplay;
    
    private static PlayerInput PlayerInput { get; } = new PlayerInput();
    private static InventoryInput InventoryInput { get; } = new InventoryInput();

    public static void SetContext(InputContext context) => CurrentContext = context;
}