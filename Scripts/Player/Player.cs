using Godot;
using GunRitualExorcistEdition.Scripts.Player.States;

public partial class Player : CharacterBody2D
{
    [Export] private CollisionShape2D _collider;
    [Export] private AnimatedSprite2D _sprite;
    [Export] private Camera2D _camera;
    [Export] private Material _material;
    [Export] private Label _playerLabel;
    public bool IsLocal { get; set; }
    public StateMachine StateMachine { get; private set; }
    public PlayerAnimator Animator { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public Vector2 InputVector { get; private set; }
    
    [Signal] public delegate void HealthChangedEventHandler(float current, float max);
    
    public override void _Ready()
    {
        Movement = new PlayerMovement(this);
        Animator = new PlayerAnimator(_sprite);
        StateMachine = new StateMachine(this);
        StateMachine.ChangeState(PlayerStateType.Idle);
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
        {
            InputVector = Movement.GetInputDirection();
            StateMachine.Update(delta);
        }
        
        Movement.UpdateDirection(InputVector);
        Animator.UpdateDirection(Movement.FacingRight);
    }
    
    public override void _PhysicsProcess(double delta)
    {
        if (IsLocal)
            StateMachine.PhysicsUpdate(delta);
    }

    public void SetDisplayName(string name)
    {
        _playerLabel.Text = name;
    }

    public void SetRemoteInput(Vector2 direction, Vector2 position, Vector2 velocity)
    {
        InputVector = direction;
        Position = position;
        Velocity = velocity;
    }
}
