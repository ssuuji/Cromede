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
            //기존 회전 목표 초기화
            stateMachine.PlayerMovement.ResetTargetRotation();

            //회피 애니메이션의 Root Motion 사용
            stateMachine.PlayerAnimation.SetRootMotion(true);

            //현재 이동 입력 방향으로 회피
            Vector3 dodgeDir = stateMachine.PlayerMovement.GetMoveDir();

            //이동 입력이 없으면 뒤쪽으로 회피
            if (dodgeDir.sqrMagnitude <= 0.001f)
            {
                dodgeDir = -stateMachine.transform.forward;
            }

            //월드 방향을 캐릭터 기준 로컬 방향으로 변환
            Vector3 localDodgeDir = stateMachine.transform.InverseTransformDirection(dodgeDir);

            //회피 방향에 맞는 애니메이션 재생
            stateMachine.PlayerAnimation.SetDodge(localDodgeDir.x, localDodgeDir.z);

            dodgeTimer = 0.0f;
        }

        public override void Update()
        {
            //현재 이동 입력값을 애니메이션에 전달
            float moveMagnitude = stateMachine.PlayerInput.MoveAction.magnitude;
            stateMachine.PlayerAnimation.SetMoveMagnitude(moveMagnitude);

            dodgeTimer += Time.deltaTime;

            //이동 입력이 있고 회피 취소 가능 시간이 지나면 이동 상태로 복귀
            if (moveMagnitude > 0.1f && dodgeTimer >= stateMachine.PlayerMovement.DodgeMoveCancelTime)
            {
                //전투 중이면 일반 이동으로 복귀
                if (stateMachine.IsCombat)
                {
                    stateMachine.ChangeState(PlayerStateType.Move);
                }
                //비전투 상태면 달리기로 복귀
                else
                {
                    stateMachine.ChangeState(PlayerStateType.Sprint);
                }

                return;
            }
        }

        public override void Exit()
        {
            //회피 상태 종료 시 Root Motion 해제
            stateMachine.PlayerAnimation.SetRootMotion(false);
        }

        public override void OnAnimationEnd()
        {
            //회피 애니메이션이 끝나면 이동 상태로 복귀
            stateMachine.ChangeState(PlayerStateType.Move);
        }
    }
}