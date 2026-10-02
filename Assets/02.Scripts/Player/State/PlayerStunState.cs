using UnityEngine;
using static Cromede.Player.State.PlayerStateMachine;

namespace Cromede.Player.State
{
    public class PlayerStunState : PlayerState
    {
        public PlayerStunState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        private float stunTimer;

        public override void Enter()
        {
            stunTimer = 0.0f;

            stateMachine.PlayerAnimation.SetStun();
        }

        public override void Update()
        {
            //스턴중 회피기 사용가능
            if (stateMachine.PlayerInput.IsSprint && stateMachine.PlayerMovement.IsGrounded)
            {
                stateMachine.ChangeState(PlayerStateType.Dodge);
                return;
            }

            stunTimer += Time.deltaTime;

            if (stunTimer >= 2.0f)
            {
                stateMachine.ChangeState(PlayerStateType.Move);
            }
        }
    }
}