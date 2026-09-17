namespace Cromede.Player.State
{
    public interface IPlayerState
    {
        void Enter();
        void Update();
        void Exit();
    }
}
