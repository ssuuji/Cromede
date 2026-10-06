using UnityEngine;
using static Cromede.Enemy.State.EnemyStateMachine;

namespace Cromede.Enemy.State
{
    public class EnemyIdleState : EnemyState
    {
        private float waitTimer;
        private bool isWandering;

        public EnemyIdleState(EnemyStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            stateMachine.ExitCombat();
            stateMachine.EnemyMovement.Stop();

            waitTimer = 0.0f;
            isWandering = false;
        }

        public override void Update()
        {
            //플레이어 감지
            if (stateMachine.HasTarget && stateMachine.TargetDistance <= stateMachine.DetectionRange)
            {
                stateMachine.ChangeState(EnemyStateType.Chase);
                return;
            }

            if (stateMachine.IdleType == EnemyIdleType.Stay)
            {
                return;
            }

            Wander();
        }

        public override void Exit()
        {
            stateMachine.EnemyMovement.Stop();
        }

        private void Wander()
        {
            if (isWandering)
            {
                if (!stateMachine.EnemyMovement.HasReachedDestination)
                {
                    return;
                }

                stateMachine.EnemyMovement.Stop();

                isWandering = false;
                waitTimer = 0.0f;

                return;
            }

            waitTimer += Time.deltaTime;

            if (waitTimer < stateMachine.WanderWaitTime)
            {
                return;
            }

            if (stateMachine.EnemyMovement.TryGetRandomPoint(stateMachine.SpawnPosition, stateMachine.WanderRadius, out Vector3 destination))
            {
                stateMachine.EnemyMovement.MoveTo(destination, stateMachine.EnemyMovement.WalkSpeed, 0.1f);
                isWandering = true;
            }

            waitTimer = 0.0f;
        }
    }
}