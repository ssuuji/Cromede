using UnityEngine;
using static Cromede.Enemy.State.EnemyStateMachine;

namespace Cromede.Enemy.State
{
    public class EnemyAttackState : EnemyState
    {
        public EnemyAttackState(EnemyStateMachine stateMachine) : base(stateMachine) { }

        private float attackTimer;
        private int comboIndex;

        public override void Enter()
        {
            attackTimer = 0.0f;
            comboIndex = 0;

            //공격 상태 진입 시 전투상태 활성화
            stateMachine.EnterCombat();

            //공격 중 이동하지 않도록 멈추기
            stateMachine.EnemyMovement.Stop();

            //타겟이 있으면 플레이어 방향 바라보기
            if (stateMachine.HasTarget)
            {
                stateMachine.EnemyMovement.FaceTarget(stateMachine.Target.position);
            }

            //공격 애니메이션 재생
            stateMachine.EnemyAnimation.SetAttack();

            //1타
            stateMachine.EnemyAttack.Attack();
        }

        public override void Update()
        {
            //1타 공격 몬스터면 추가 공격 처리하지 않음
            if (stateMachine.EnemyAttack.AttackCount <= 1)
            {
                return;
            }

            attackTimer += Time.deltaTime;

            //설정된 콤보 간격이 지나면 2타
            if (comboIndex == 0 && attackTimer >= stateMachine.EnemyAttack.ComboInterval)
            {
                comboIndex = 1;
                stateMachine.EnemyAttack.Attack();
            }
        }

        public override void Exit()
        {
            //공격 상태가 끝나면 공격 쿨타임 시작
            stateMachine.EnemyAttack.StartCooldown();
        }

        public override void OnAnimationEnd()
        {
            //공격 애니메이션이 끝나면 다시 플레이어 추적
            stateMachine.ChangeState(EnemyStateType.Chase);
        }
    }
}