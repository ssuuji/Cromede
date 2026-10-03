using UnityEngine;
using static Cromede.Player.State.PlayerStateMachine;

namespace Cromede.Player.State
{
    public class PlayerJumpState : PlayerState
    {
        public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        private bool isSprintJump;

        public override void Enter()
        {
            if (isSprintJump)
            {
                stateMachine.PlayerAnimation.SetSprintJump();
            }
            else
            {
                stateMachine.PlayerAnimation.SetJump();
            }

            stateMachine.PlayerMovement.Jump();
        }

        public override void Update()
        {
            Transform currentTarget = stateMachine.PlayerTargeting.CurrentTarget;

            stateMachine.PlayerMovement.JumpMove(true, currentTarget);

            //착지
            if (stateMachine.PlayerMovement.IsGrounded)
            {
                bool isMove = stateMachine.PlayerInput.MoveAction.sqrMagnitude > 0.001f;

                //이동 입력 여부에 따라 착지 애니메이션 구분
                if (isMove)
                {
                    stateMachine.PlayerAnimation.SetJumpLandRun();
                }
                else
                {
                    stateMachine.PlayerAnimation.SetJumpLand();
                }

                //질주 중 시작한 점프이고 이동 입력이 유지되면 다시 질주
                if (isSprintJump && isMove)
                {
                    stateMachine.ChangeState(PlayerStateType.Sprint);
                    return;
                }

                //일반 이동으로 복귀
                stateMachine.ChangeState(PlayerStateType.Move);
                return;
            }
        }

        public override void Exit()
        {
            isSprintJump = false;
        }

        //질주 점프 여부 설정
        public void SetSprintJump(bool isSprintJump)
        {
            this.isSprintJump = isSprintJump;
        }
    }
}