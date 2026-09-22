using UnityEngine;
using static Cromede.Player.State.PlayerStateMachine;

namespace Cromede.Player.State
{
    public class PlayerDodgeState : PlayerState
    {
        public PlayerDodgeState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        private float dodgeTimer;

        public override void Enter()
        {
            stateMachine.PlayerMovement.ResetTargetRotation();
            stateMachine.PlayerAnimation.SetRootMotion(true);

            Vector3 dodgeDir = stateMachine.PlayerMovement.GetMoveDir();

            if (dodgeDir.sqrMagnitude <= 0.001f)
            {
                dodgeDir = -stateMachine.transform.forward;
            }

            //애니메이션
            Vector3 localDodgeDir = stateMachine.transform.InverseTransformDirection(dodgeDir); //월드방향 dodgeDir을 캐릭터기준 로컬방향으로 전환
            stateMachine.PlayerAnimation.SetDodge(localDodgeDir.x, localDodgeDir.z);

            dodgeTimer = 0.0f;
        }
        public override void Update()
        {
            //움직임여부 전송
            float moveMagnitude = stateMachine.PlayerInput.MoveAction.magnitude;
            stateMachine.PlayerAnimation.SetMoveMagnitude(moveMagnitude);
            
            dodgeTimer += Time.deltaTime;

            // 이동 입력이 있으면 Recovery 구간 전에 바로 달리기로 복귀
            if (moveMagnitude > 0.1f && dodgeTimer >= stateMachine.PlayerMovement.DodgeMoveCancelTime)
            {
                stateMachine.ChangeState(PlayerStateType.Sprint);
                return;
            }

            // 이동 입력이 없으면 Dodge 전체 재생
            if (dodgeTimer >= stateMachine.PlayerMovement.DodgeDuration)
            {
                stateMachine.ChangeState(PlayerStateType.Move);
            }
        }

        public override void Exit()
        {
            stateMachine.PlayerAnimation.SetRootMotion(false);
        }
    }
}