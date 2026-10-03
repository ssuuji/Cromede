using UnityEngine;
using static Cromede.Player.State.PlayerStateMachine;

namespace Cromede.Player.State
{
    public class PlayerDieState : PlayerState
    {
        public PlayerDieState(PlayerStateMachine stateMachine) : base(stateMachine) { }
        public override void Enter()
        {
            stateMachine.PlayerAnimation.SetDie();
        }

        public override void Update()
        {
            stateMachine.PlayerMovement.VerticalMove();
        }
    }
}