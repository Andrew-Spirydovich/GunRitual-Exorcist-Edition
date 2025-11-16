namespace GunRitualExorcistEdition.Scripts.Player;

public interface IAttacker
{
    State<Characters.Character> Attack();
}