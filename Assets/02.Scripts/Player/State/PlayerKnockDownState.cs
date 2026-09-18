using UnityEngine;
using static Cromede.Player.State.PlayerStateMachine;

namespace Cromede.Player.State
{
    public class PlayerKnockDownState : PlayerState
    {
        public PlayerKnockDownState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        private float knockDownTimer;

        public override void Enter()
        {
            knockDownTimer = 0.0f;
        }

        public override void Update()
        {
            if (stateMachine.PlayerInput.IsSprint && stateMachine.PlayerMovement.IsGrounded)
            {
                stateMachine.ChangeState(PlayerStateType.Dodge);
                return;
            }

            knockDownTimer += Time.deltaTime;

            if (knockDownTimer >= 2.0f)
            {
                stateMachine.ChangeState(PlayerStateType.Move);
            }
        }
    }
}
