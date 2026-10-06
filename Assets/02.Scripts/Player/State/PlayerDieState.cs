using UnityEngine;
using static Cromede.Player.State.PlayerStateMachine;

namespace Cromede.Player.State
{
    public class PlayerDieState : PlayerState
    {
        public PlayerDieState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            //사망 애니메이션 재생
            stateMachine.PlayerAnimation.SetDie();
        }

        public override void Update()
        {
            //사망 중에도 중력 적용
            stateMachine.PlayerMovement.VerticalMove();
        }
    }
}