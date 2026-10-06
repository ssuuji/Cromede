using UnityEngine;
using static Cromede.Player.State.PlayerStateMachine;

namespace Cromede.Player.State
{
    public class PlayerKnockDownState : PlayerState
    {
        public PlayerKnockDownState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        private float knockDownTimer; 
        private bool isKnockDownEnd;  

        public override void Enter()
        {
            knockDownTimer = 0.0f;
            isKnockDownEnd = false;

            //넉다운 애니메이션 재생
            stateMachine.PlayerAnimation.SetKnockDown();
        }

        public override void Update()
        {
            //넉다운 중에도 중력 적용
            stateMachine.PlayerMovement.VerticalMove();

            //넉다운 중 회피 입력이 들어오면 회피 상태로 전환
            if (stateMachine.PlayerInput.IsSprint && stateMachine.PlayerMovement.IsGrounded)
            {
                stateMachine.ChangeState(PlayerStateType.Dodge);
                return;
            }

            knockDownTimer += Time.deltaTime;

            //2초 지나면 일어나는 애니메이션 재생
            if (!isKnockDownEnd && knockDownTimer >= 2.0f)
            {
                isKnockDownEnd = true;
                stateMachine.PlayerAnimation.SetKnockDownEnd();
            }
        }

        public override void OnAnimationEnd()
        {
            //넉다운 종료 애니메이션이 끝난 경우에만 이동 상태로 복귀
            if (!isKnockDownEnd)
            {
                return;
            }

            stateMachine.ChangeState(PlayerStateType.Move);
        }
    }
}