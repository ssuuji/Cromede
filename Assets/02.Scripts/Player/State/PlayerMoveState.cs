namespace Cromede.Player.State
{
    public class PlayerMoveState : IPlayerState
    {
        private PlayerStateMachine stateMachine;


        public PlayerMoveState(PlayerStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public void Enter()
        {

        }

        public void Update()
        {
            stateMachine.PlayerMovement.Move();

            if (stateMachine.PlayerInput.IsSprint && stateMachine.PlayerMovement.IsGrounded)
            {
                stateMachine.ChangeState(stateMachine.DodgeState);
            }
        }

        public void Exit()
        {

        }
    }
}
