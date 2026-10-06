using UnityEngine;
using static Cromede.Player.State.PlayerStateMachine;

namespace Cromede.Player.State
{
    public class PlayerAttackState : PlayerState
    {
        public PlayerAttackState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        private float attackTimer; 
        private int comboIndex;    //현재 콤보 순서
        private bool nextAttack;   //다음 콤보 입력 예약

        public override void Enter()
        {
            //첫 공격부터 시작
            comboIndex = 0;
            StartAttack();
        }

        public override void Update()
        {
            //공격 중 점프
            if (stateMachine.PlayerInput.IsJump && stateMachine.PlayerMovement.IsGrounded)
            {
                stateMachine.ChangeJumpState(false);
                return;
            }

            //공격 중 회피
            if (stateMachine.PlayerInput.IsSprint && stateMachine.PlayerMovement.IsGrounded)
            {
                stateMachine.ChangeState(PlayerStateType.Dodge);
                return;
            }

            //공격 중에도 이동 가능
            stateMachine.PlayerMovement.Move(false);

            //공격 중 이동 애니메이션 갱신
            stateMachine.PlayerAnimation.SetMoveMagnitude(stateMachine.PlayerInput.MoveAction.magnitude);

            //이동 방향을 플레이어 기준 방향으로 변환
            Vector3 moveDir = stateMachine.PlayerMovement.GetMoveDir();
            Vector3 localMoveDir = stateMachine.transform.InverseTransformDirection(moveDir);

            //이동 방향 애니메이션 갱신
            stateMachine.PlayerAnimation.SetMoveDir(localMoveDir.x, localMoveDir.z);

            attackTimer += Time.deltaTime;

            //공격 중 다시 공격하면 다음 콤보 예약
            if (stateMachine.PlayerInput.IsAttack)
            {
                nextAttack = true;
            }

            //콤보 입력이 있고 콤보 전환 시간이 지나면 다음 공격
            if ((nextAttack || stateMachine.PlayerInput.IsAttackHeld) && attackTimer >= stateMachine.PlayerAttack.ComboInterval)
            {
                //다음 콤보 순서로 변경
                if (comboIndex < 2)
                {
                    comboIndex++;
                }
                else
                {
                    comboIndex = 0;
                }

                StartAttack();
                return;
            }
        }

        public override void Exit()
        {
            //공격 상태 종료 후 전투상태 종료 타이머 시작
            stateMachine.StartCIdleTimer();
        }

        public override void OnAnimationEnd()
        {
            //공격 애니메이션이 끝나면 이동 상태로 전환
            stateMachine.ChangeState(PlayerStateType.Move);
        }

        //공격 시작
        private void StartAttack()
        {
            attackTimer = 0.0f;
            nextAttack = false;

            //전투상태 활성화
            stateMachine.EnterCombat();

            //공격 애니메이션 재생
            stateMachine.PlayerAnimation.SetAttack();

            //공격 판정 실행
            stateMachine.PlayerAttack.Attack();
        }
    }
}