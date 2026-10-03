using static Cromede.Player.State.PlayerStateMachine;

namespace Cromede.Player.State
{
    public class PlayerRebirthState : PlayerState
    {
        public PlayerRebirthState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            stateMachine.PlayerAnimation.SetRebirth();
        }

        public override void Update()
        {
            stateMachine.PlayerMovement.VerticalMove();
        }

        public override void OnAnimationEnd()
        {
            stateMachine.ChangeState(PlayerStateType.Move);
        }
    }
}