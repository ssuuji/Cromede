using static Cromede.Player.State.PlayerStateMachine;

namespace Cromede.Player.State
{
    public class PlayerSprintState : PlayerState
    {
        public PlayerSprintState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            stateMachine.PlayerAnimation.SetSprint();
        }


        public override void Update()
        {
            //점프
            if (stateMachine.PlayerInput.IsJump && stateMachine.PlayerMovement.IsGrounded)
            {
                stateMachine.PlayerAnimation.SetSprintJump();
                stateMachine.ChangeJumpState(true);
                return;
            }

            //회피
            if (stateMachine.PlayerInput.IsSprint && stateMachine.PlayerMovement.IsGrounded)
            {
                stateMachine.ChangeState(PlayerStateType.Dodge);
                return;
            }
            
            //이동
            if (stateMachine.PlayerInput.MoveAction.sqrMagnitude <= 0.001f)
            {
                stateMachine.PlayerAnimation.SetSprintStop();
                stateMachine.ChangeState(PlayerStateType.Move);
                return;
            }

            stateMachine.PlayerMovement.Sprint();
        }

    }
}
