using static Cromede.Player.State.PlayerStateMachine;

namespace Cromede.Player.State
{
    public class PlayerStunState : PlayerState
    {
        public PlayerStunState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            stateMachine.PlayerAnimation.SetStun();
            stateMachine.PlayerAnimation.SetStunWeapon();
        }

        public override void Update()
        {
            stateMachine.PlayerMovement.VerticalMove();

            //스턴중 회피기 사용가능
            if (stateMachine.PlayerInput.IsSprint && stateMachine.PlayerMovement.IsGrounded)
            {
                stateMachine.ChangeState(PlayerStateType.Dodge);
                return;
            }
        }

        public override void Exit()
        {
            stateMachine.PlayerAnimation.ResetStunWeapon();
        }


        public override void OnAnimationEnd()
        {
            stateMachine.ChangeState(PlayerStateType.Move);
        }
    }
}