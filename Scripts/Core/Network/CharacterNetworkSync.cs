using GunRitualExorcistEdition.Scripts.Characters;

namespace GunRitualExorcistEdition.Scripts.Core.Network;

public class CharacterNetworkSync
{
    private Character _character;
    private float _sendTimer;
    private const float SEND_INTERVAL = 0.01f;

    public void Initialize(Character character)
    {
        _character = character;
    }

    public void Tick(double delta)
    {
        if (_character == null) 
            return;
        
        if (_character.ControlMode != ControlMode.Local) 
            return;

        _sendTimer += (float)delta;
        
        if (_sendTimer < SEND_INTERVAL) 
            return;
        
        _sendTimer = 0f;

        NetworkClient.Instance.SendMoveRequest(BuildMoveDto());
    }
    
    public void SendImmediate()
    {
        if (_character.ControlMode != ControlMode.Local) 
            return; 
        
        NetworkClient.Instance.SendMoveRequest(BuildMoveDto());
    }
    
    public void SendStateImmediate()
    {
        if (_character.ControlMode != ControlMode.Local) 
            return;

        NetworkClient.Instance.SendStateRequest(
            NetworkClient.Instance.LocalUserID,
            _character.StateMachine?.CurrentStateName ?? "IdleState"
        );
    }

    private PlayerMoveDto BuildMoveDto()
    {
        return new PlayerMoveDto
        {
            Id = NetworkClient.Instance.LocalUserID,
            X = _character.GlobalPosition.X,
            Y = _character.GlobalPosition.Y,
            DirX = _character.InputVector.X,
            DirY = _character.InputVector.Y,
            State = _character.StateMachine?.CurrentStateName ?? "Idle"
        };
    }
}