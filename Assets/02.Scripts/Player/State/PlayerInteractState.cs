using UnityEngine;

namespace Cromede.Player.State
{
    public class PlayerInteractState : PlayerState
    {
        public PlayerInteractState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        private float interfactTimer;

        public override void Enter()
        {
            interfactTimer = 0.0f;

            Debug.Log("상호작용 엔터");
        }
        public override void Update()
        {
            //상호작용중 움직이면 상호작용 취소
            if (stateMachine.PlayerInput.MoveAction.sqrMagnitude > 0.001f)
            {
                Debug.Log("상호작용 취소");
                stateMachine.ChangeState(stateMachine.MoveState);
                return;
            }

            interfactTimer += Time.deltaTime;

            if (interfactTimer >= 3.0f)
            {
                stateMachine.ChangeState(stateMachine.MoveState);
            }
        }

        public override void Exit()
        {
            Debug.Log("상호작용 끗");
        }
    }
}