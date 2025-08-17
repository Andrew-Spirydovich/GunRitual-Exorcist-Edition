using Godot;

public partial class Player : Node2D
{
    [Export] private CollisionShape2D _collider;
    [Export] private AnimatedSprite2D _sprite;
    [Export] private Camera2D _camera;
    [Export] private Material _material;
    
    public bool IsLocal { get; set; }
    public StateMachine StateMachine { get; private set; }
    public PlayerAnimator Animator { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public Vector2 InputVector { get; private set; }
    
    public override void _Ready()
    {
        Movement = new PlayerMovement(_collider, this);
        Animator = new PlayerAnimator(_sprite);
        StateMachine = new StateMachine();
        StateMachine.ChangeState(new IdleState(this));
        SetProcess(true);

        if (IsLocal)
        {
            _sprite.Material = _material;
            _camera.Enabled = true;
        }
    }   

    public override void _Process(double delta)
    {
        if (IsLocal)
            InputVector = Movement.GetInputDirection();
        
        StateMachine?.Update(delta);
        Animator?.UpdateDirection(InputVector);
    }
    
    public override void _PhysicsProcess(double delta)
    {
        if (IsLocal)
            StateMachine?.PhysicsUpdate(delta);
    }

    public void SetRemoteInput(Vector2 direction, Vector2 position)
    {
        InputVector = direction;
        Position = position;
    }
}
