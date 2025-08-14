using Godot;
using System;

public class StateMachine
{
    public PlayerState CurrentState { get; private set; }
    
    public void ChangeState(PlayerState newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Update(double delta)
    {
        CurrentState?.Update(delta);
    }

    public void PhysicsUpdate(double delta)
    {
        CurrentState?.PhysicsUpdate(delta);
    }
}
