using UnityEngine;
using static Cromede.Player.State.PlayerStateMachine;

namespace Cromede.Player.State
{
    public class PlayerHitState : PlayerState
    {
        public PlayerHitState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Update()
        {
            stateMachine.ChangeState(PlayerStateType.Move);
        }
    }
}

