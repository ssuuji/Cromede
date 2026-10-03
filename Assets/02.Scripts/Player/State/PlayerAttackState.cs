using UnityEngine;
using static Cromede.Player.State.PlayerStateMachine;

namespace Cromede.Player.State
{
    public class PlayerAttackState : PlayerState
    {
        public PlayerAttackState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        private float attackTimer;
        private int comboIndex;
        private bool nextAttack;

        public override void Enter()
        {
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

            //공격하면서 움직일 수 있어야함
            stateMachine.PlayerMovement.Move(false);

            //공격 중 이동 애니메이션 갱신
            stateMachine.PlayerAnimation.SetMoveMagnitude(stateMachine.PlayerInput.MoveAction.magnitude);

            Vector3 moveDir = stateMachine.PlayerMovement.GetMoveDir();
            Vector3 localMoveDir = stateMachine.transform.InverseTransformDirection(moveDir);

            stateMachine.PlayerAnimation.SetMoveDir(localMoveDir.x, localMoveDir.z);

            attackTimer += Time.deltaTime;

            //공격 중 다시 공격클릭 하면 다음 콤보 예약
            if (stateMachine.PlayerInput.IsAttack)
            {
                nextAttack = true;
            }

            //콤보 입력이 있고 콤보 전환 시간이 지나면 다음 공격
            if ((nextAttack || stateMachine.PlayerInput.IsAttackHeld) && attackTimer >= stateMachine.PlayerAttack.ComboInterval)
            {
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
            //공격 상태 종료 후 전투 대기 시간 시작
            stateMachine.StartCIdleTimer();
        }

        public override void OnAnimationEnd()
        {
            stateMachine.ChangeState(PlayerStateType.Move);
        }

        //공격
        private void StartAttack()
        {
            attackTimer = 0.0f;
            nextAttack = false;

            stateMachine.EnterCombat();
            stateMachine.PlayerAnimation.SetAttack();
            stateMachine.PlayerAttack.Attack();
        }
    }

}
