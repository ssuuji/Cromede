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
            if (stateMachine.PlayerInput.IsSprint && stateMachine.PlayerMovement.IsGrounded)
            {
                stateMachine.ChangeState(PlayerStateType.Dodge);
                return;
            }

            knockDownTimer += Time.deltaTime;

            if (!isKnockDownEnd && knockDownTimer >= 2.0f)
            {
                isKnockDownEnd = true;
                knockDownTimer = 0.0f;

                stateMachine.PlayerAnimation.SetKnockDownEnd();
                return;
            }

            if (isKnockDownEnd && knockDownTimer >= 1.375f)
            {
                stateMachine.ChangeState(PlayerStateType.Move);
            }
        }
    }
}
