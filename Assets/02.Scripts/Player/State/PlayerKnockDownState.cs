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

            stateMachine.PlayerAnimation.SetKnockDown();
        }

        public override void Update()
        {
            stateMachine.PlayerMovement.VerticalMove();

            if (stateMachine.PlayerInput.IsSprint && stateMachine.PlayerMovement.IsGrounded)
            {
                stateMachine.ChangeState(PlayerStateType.Dodge);
                return;
            }

            knockDownTimer += Time.deltaTime;

            if (!isKnockDownEnd && knockDownTimer >= 2.0f)
            {
                isKnockDownEnd = true;

                stateMachine.PlayerAnimation.SetKnockDownEnd();
            }
        }

        public override void OnAnimationEnd()
        {
            if (!isKnockDownEnd) return;

            stateMachine.ChangeState(PlayerStateType.Move);
        }
    }
}