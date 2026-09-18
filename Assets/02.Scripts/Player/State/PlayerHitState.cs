using UnityEngine;

namespace Cromede.Player.State
{
    public class PlayerHitState : PlayerState
    {
        public PlayerHitState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Update()
        {
            stateMachine.ChangeState(stateMachine.MoveState);
        }
    }
}

