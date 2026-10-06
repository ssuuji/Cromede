using static Cromede.Enemy.State.EnemyStateMachine;

namespace Cromede.Enemy.State
{
    public class EnemyDormantState : EnemyState
    {
        private bool isFirstEnter = true;

        public EnemyDormantState(EnemyStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            stateMachine.ExitCombat();
            stateMachine.EnemyMovement.Stop();

            //게임 시작 시 Animator 기본 State가 이미 Sidle_Start
            if (isFirstEnter)
            {
                isFirstEnter = false;
                return;
            }

            //복귀 후 다시 석상 상태
            stateMachine.EnemyAnimation.SetDormant();
        }

        public override void Update()
        {
            if (!stateMachine.HasTarget)
            {
                return;
            }

            if (stateMachine.TargetDistance > stateMachine.DetectionRange)
            {
                return;
            }

            stateMachine.ChangeState(EnemyStateType.Wake);
        }
    }
}