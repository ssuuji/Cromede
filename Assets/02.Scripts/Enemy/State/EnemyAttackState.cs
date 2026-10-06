using UnityEngine;
using static Cromede.Enemy.State.EnemyStateMachine;

namespace Cromede.Enemy.State
{
    public class EnemyAttackState : EnemyState
    {
        private float attackTimer;
        private int comboIndex;

        public EnemyAttackState(EnemyStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            attackTimer = 0.0f;
            comboIndex = 0;

            stateMachine.EnterCombat();
            stateMachine.EnemyMovement.Stop();

            if (stateMachine.HasTarget)
            {
                stateMachine.EnemyMovement.FaceTarget(stateMachine.Target.position);
            }

            stateMachine.EnemyAnimation.SetAttack();

            //1타
            stateMachine.EnemyAttack.Attack();
        }

        public override void Update()
        {
            if (stateMachine.EnemyAttack.AttackCount <= 1)
            {
                return;
            }

            attackTimer += Time.deltaTime;

            //2타
            if (comboIndex == 0 && attackTimer >= stateMachine.EnemyAttack.ComboInterval)
            {
                comboIndex = 1;
                stateMachine.EnemyAttack.Attack();
            }
        }

        public override void Exit()
        {
            stateMachine.EnemyAttack.StartCooldown();
        }

        public override void OnAnimationEnd()
        {
            stateMachine.ChangeState(EnemyStateType.Chase);
        }
    }
}