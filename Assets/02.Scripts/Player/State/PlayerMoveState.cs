using UnityEngine;
using static Cromede.Player.State.PlayerStateMachine;

namespace Cromede.Player.State
{
    public class PlayerMoveState : PlayerState
    {
        public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Update()
        {
            //점프
            if (stateMachine.PlayerInput.IsJump && stateMachine.PlayerMovement.IsGrounded)
            {
                stateMachine.ChangeJumpState(false);
                return;
            }

            //회피
            if (stateMachine.PlayerInput.IsSprint && stateMachine.PlayerMovement.IsGrounded)
            {
                stateMachine.ChangeState(PlayerStateType.Dodge);
                return;
            }

            //공격
            if (stateMachine.PlayerInput.IsAttack)
            {
                //공격 전 현재 이동값을 애니메이션에 전달
                stateMachine.PlayerAnimation.SetMoveMagnitude(stateMachine.PlayerInput.MoveAction.magnitude);

                stateMachine.ChangeState(PlayerStateType.Attack);
                return;
            }

            //상호작용
            if (stateMachine.PlayerInput.IsInteract)
            {
                //상호작용 가능한 대상이 있으면 상호작용 상태로 전환
                if (stateMachine.PlayerInteract.Interact())
                {
                    stateMachine.ChangeState(PlayerStateType.Interact);
                }

                return;
            }

            //현재 이동 입력값을 애니메이션에 전달
            stateMachine.PlayerAnimation.SetMoveMagnitude(stateMachine.PlayerInput.MoveAction.magnitude);

            //현재 타겟 가져오기
            Transform currentTarget = stateMachine.PlayerTargeting.CurrentTarget;

            //타겟이 없고 비전투 상태면 항상 정면 이동 애니메이션 사용
            if (currentTarget == null && !stateMachine.IsCombat)
            {
                stateMachine.PlayerAnimation.SetMoveDir(0.0f, 1.0f);
            }
            else
            {
                //이동 방향을 플레이어 기준 로컬 방향으로 변환
                Vector3 moveDir = stateMachine.PlayerMovement.GetMoveDir();
                Vector3 localMoveDir = stateMachine.transform.InverseTransformDirection(moveDir);

                //이동 방향에 맞는 애니메이션 갱신
                stateMachine.PlayerAnimation.SetMoveDir(localMoveDir.x, localMoveDir.z);
            }

            //실제 플레이어 이동 처리
            stateMachine.PlayerMovement.Move(true, currentTarget);
        }
    }
}