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

            Debug.Log("스턴 엔터");
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

            if (stunTimer >= 1.0f)
            {
                stateMachine.ChangeState(PlayerStateType.Move);
            }
        }
    }
}