namespace Cromede.Player.State
{
    public class PlayerSprintState : PlayerState
    {
        public PlayerSprintState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Update()
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
    }
}
