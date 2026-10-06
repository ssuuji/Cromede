using static Cromede.Enemy.State.EnemyStateMachine;

namespace Cromede.Enemy.State
{
    public class EnemyChaseState : EnemyState
    {
        public EnemyChaseState(EnemyStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            stateMachine.EnterCombat();
        }

        public override void Update()
        {
            if (!stateMachine.HasTarget)
            {
                stateMachine.ChangeState(EnemyStateType.Return);
                return;
            }

            //최대 추적 거리 초과
            if (stateMachine.SpawnDistance > stateMachine.ReturnRange)
            {
                stateMachine.ChangeState(EnemyStateType.Return);
                return;
            }

            //공격 거리
            if (stateMachine.TargetDistance <= stateMachine.EnemyAttack.AttackRange)
            {
                stateMachine.EnemyMovement.Stop();
                stateMachine.EnemyMovement.FaceTarget(stateMachine.Target.position);

                if (stateMachine.EnemyAttack.CanAttack)
                {
                    stateMachine.ChangeState(EnemyStateType.Attack);
                }

                return;
            }

            stateMachine.EnemyMovement.MoveTo(stateMachine.Target.position, stateMachine.EnemyMovement.RunSpeed, stateMachine.EnemyMovement.ChaseStoppingDistance);
        }
    }
}