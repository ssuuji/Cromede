using static Cromede.Player.State.PlayerStateMachine;

namespace Cromede.Player.State
{
    public class PlayerStunState : PlayerState
    {
        public PlayerStunState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            //스턴 애니메이션 재생
            stateMachine.PlayerAnimation.SetStun();

            //스턴 상태에 맞게 무기 애니메이션 변경
            stateMachine.PlayerAnimation.SetStunWeapon();
        }

        public override void Update()
        {
            //스턴 중에도 중력 적용
            stateMachine.PlayerMovement.VerticalMove();

            //스턴 중 회피 사용 가능
            if (stateMachine.PlayerInput.IsSprint && stateMachine.PlayerMovement.IsGrounded)
            {
                stateMachine.ChangeState(PlayerStateType.Dodge);
                return;
            }
        }

        public override void Exit()
        {
            //스턴 상태가 끝나면 무기 애니메이션 원상복구
            stateMachine.PlayerAnimation.ResetStunWeapon();
        }

        public override void OnAnimationEnd()
        {
            //스턴 애니메이션이 끝나면 이동 상태로 전환
            stateMachine.ChangeState(PlayerStateType.Move);
        }
    }
}