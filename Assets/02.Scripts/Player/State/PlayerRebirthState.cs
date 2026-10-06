using static Cromede.Player.State.PlayerStateMachine;

namespace Cromede.Player.State
{
    public class PlayerRebirthState : PlayerState
    {
        public PlayerRebirthState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            //부활 애니메이션 재생
            stateMachine.PlayerAnimation.SetRebirth();
        }

        public override void Update()
        {
            //부활 중에도 중력 적용
            stateMachine.PlayerMovement.VerticalMove();
        }

        public override void OnAnimationEnd()
        {
            //부활 애니메이션이 끝나면 이동 상태로 전환
            stateMachine.ChangeState(PlayerStateType.Move);
        }
    }
}