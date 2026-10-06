using static Cromede.Player.State.PlayerStateMachine;

namespace Cromede.Player.State
{
    public class PlayerSprintState : PlayerState
    {
        public PlayerSprintState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            //질주 애니메이션 재생
            stateMachine.PlayerAnimation.SetSprint();
        }

        public override void Update()
        {
            //공격
            if (stateMachine.PlayerInput.IsAttack)
            {
                //공격 전 현재 이동값을 애니메이션에 전달
                stateMachine.PlayerAnimation.SetMoveMagnitude(stateMachine.PlayerInput.MoveAction.magnitude);

                stateMachine.ChangeState(PlayerStateType.Attack);
                return;
            }

            //점프
            if (stateMachine.PlayerInput.IsJump && stateMachine.PlayerMovement.IsGrounded)
            {
                //질주 중 점프로 설정
                stateMachine.ChangeJumpState(true);
                return;
            }

            //회피
            if (stateMachine.PlayerInput.IsSprint && stateMachine.PlayerMovement.IsGrounded)
            {
                stateMachine.ChangeState(PlayerStateType.Dodge);
                return;
            }

            //이동 입력이 없으면 질주 종료
            if (stateMachine.PlayerInput.MoveAction.sqrMagnitude <= 0.001f)
            {
                stateMachine.PlayerAnimation.SetSprintStop();
                stateMachine.ChangeState(PlayerStateType.Move);
                return;
            }

            //질주 이동
            stateMachine.PlayerMovement.Sprint();
        }
    }
}