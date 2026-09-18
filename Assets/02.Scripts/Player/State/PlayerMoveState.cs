namespace Cromede.Player.State
{
    public class PlayerMoveState : PlayerState
    {
        public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Update()
        {
            //회피
            if (stateMachine.PlayerInput.IsSprint && stateMachine.PlayerMovement.IsGrounded)
            {
                stateMachine.ChangeState(stateMachine.DodgeState);
                return;
            }

            //공격
            if (stateMachine.PlayerInput.IsAttack)
            {
                stateMachine.ChangeState(stateMachine.AttackState);
                return;
            }

            //상호작용
            if (stateMachine.PlayerInput.IsInteract)
            {
                stateMachine.PlayerInteract.Interact();
                return;
            }

            stateMachine.PlayerMovement.Move(true);
        }
    }
}
