using static Cromede.Enemy.State.EnemyStateMachine;

namespace Cromede.Enemy.State
{
    public class EnemyReturnState : EnemyState
    {
        public EnemyReturnState(EnemyStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            stateMachine.ExitCombat();

            stateMachine.EnemyMovement.MoveTo(
                stateMachine.SpawnPosition,
                stateMachine.EnemyMovement.RunSpeed,
                0.0f);
        }

        public override void Update()
        {
            //복귀 완료
            if (stateMachine.EnemyMovement.HasReachedDestination)
            {
                stateMachine.EnemyMovement.Stop();

                if (stateMachine.StartType == EnemyStartType.Dormant)
                {
                    stateMachine.ChangeState(EnemyStateType.Dormant);
                    return;
                }

                stateMachine.ChangeState(EnemyStateType.Idle);
                return;
            }

            stateMachine.EnemyMovement.MoveTo(
                stateMachine.SpawnPosition,
                stateMachine.EnemyMovement.RunSpeed,
                0.0f);
        }
    }
}