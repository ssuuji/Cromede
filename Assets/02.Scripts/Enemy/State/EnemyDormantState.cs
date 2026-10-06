using static Cromede.Enemy.State.EnemyStateMachine;

namespace Cromede.Enemy.State
{
    public class EnemyDormantState : EnemyState
    {
        public EnemyDormantState(EnemyStateMachine stateMachine) : base(stateMachine) { }

        private bool isFirstEnter = true; //게임 시작 진입인지 복귀 진입인지 구분

        public override void Enter()
        {
            //석상상태에서는 전투해제
            stateMachine.ExitCombat();

            //이동멈추기
            stateMachine.EnemyMovement.Stop();

            //게임 시작 시 기본 상태가 이미 Sidle_Start라서 Dormant 트리거 쏘지않음
            if (isFirstEnter)
            {
                isFirstEnter = false;
                return;
            }

            //복귀 후 다시 석상 상태로 변경
            stateMachine.EnemyAnimation.SetDormant();
        }

        public override void Update()
        {
            //타겟이 없으면 그대로 대기
            if (!stateMachine.HasTarget)
            {
                return;
            }

            //플레이어가 감지 범위 밖이면 그대로 대기
            if (stateMachine.TargetDistance > stateMachine.DetectionRange)
            {
                return;
            }

            //플레이어가 감지 범위 안에 들어오면 깨어남 상태로 전환
            stateMachine.ChangeState(EnemyStateType.Wake);
        }
    }
}