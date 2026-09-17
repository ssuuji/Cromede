using UnityEngine;

namespace Cromede.Player.State
{
    public class PlayerSprintState : IPlayerState
    {
        private PlayerStateMachine stateMachine;

        public PlayerSprintState(PlayerStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public void Enter()
        {

        }

        public void Update()
        {
            if (stateMachine.PlayerInput.IsSprint && stateMachine.PlayerMovement.IsGrounded)
            {
                stateMachine.ChangeState(stateMachine.DodgeState);
                return;
            }

            if (stateMachine.PlayerInput.MoveAction.sqrMagnitude <= 0.001f)
            {
                stateMachine.ChangeState(stateMachine.MoveState);
                return;
            }

            stateMachine.PlayerMovement.Sprint();
        }

        public void Exit()
        {

        }
    }

}
