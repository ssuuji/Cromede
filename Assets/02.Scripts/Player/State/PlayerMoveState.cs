using static Cromede.Player.State.PlayerStateMachine;

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
                stateMachine.ChangeState(PlayerStateType.Dodge);
                return;
            }

            //공격
            if (stateMachine.PlayerInput.IsAttack)
            {
                stateMachine.ChangeState(PlayerStateType.Attack);
                return;
            }

            //상호작용
            if (stateMachine.PlayerInput.IsInteract)
            {
                stateMachine.PlayerInteract.Interact();
                return;
            }

            //애니메이션
            stateMachine.PlayerAnimation.SetMoveMagnitude(stateMachine.PlayerInput.MoveAction.magnitude);

            stateMachine.PlayerMovement.Move(true);
        }
    }
}
