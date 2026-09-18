using UnityEngine;

namespace Cromede.Player.State
{
    public class PlayerDodgeState : PlayerState
    {
        public PlayerDodgeState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        private Vector3 dodgeDir;
        private float dodgeTimer;

        public override void Enter()
        {
            dodgeDir = stateMachine.PlayerMovement.GetMoveDir();

            if (dodgeDir.sqrMagnitude <= 0.001f)
            {
                dodgeDir = stateMachine.transform.forward;
            }

            dodgeTimer = 0.0f;
        }
        public override void Update()
        {
            stateMachine.PlayerMovement.Dodge(dodgeDir);

            dodgeTimer += Time.deltaTime;

            if (dodgeTimer >= stateMachine.PlayerMovement.DodgeDuration)
            {
                stateMachine.ChangeState(stateMachine.SprintState);
            }
        }
    }
}

