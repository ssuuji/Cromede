using UnityEngine;
using static Cromede.Player.State.PlayerStateMachine;

namespace Cromede.Player.State
{
    public class PlayerRebirthState : PlayerState
    {
        public PlayerRebirthState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        private float rebirthTimer;

        public override void Enter()
        {
            rebirthTimer = 0.0f;

            stateMachine.PlayerAnimation.SetRebirth();
        }

        public override void Update()
        {
            rebirthTimer += Time.deltaTime;

            if (rebirthTimer >= 2.458f)
            {
                stateMachine.ChangeState(PlayerStateType.Move);
            }
        }
    }
}