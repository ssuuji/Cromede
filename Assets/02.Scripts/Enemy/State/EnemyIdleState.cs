using UnityEngine;
using static Cromede.Enemy.State.EnemyStateMachine;

namespace Cromede.Enemy.State
{
    public class EnemyIdleState : EnemyState
    {
        public EnemyIdleState(EnemyStateMachine stateMachine) : base(stateMachine) { }

        private float waitTimer; //다음 배회까지 대기시간
        private bool isWander;   //배회중인지 여부

        public override void Enter()
        {
            //대기상태에서는 전투해제
            stateMachine.ExitCombat();
            //이동멈추기
            stateMachine.EnemyMovement.Stop();

            waitTimer = 0.0f;
            isWander = false;
        }

        public override void Update()
        {
            //플레이어가 감지 범위 안에 들어오면 추적 상태로 전환
            if (stateMachine.HasTarget && stateMachine.TargetDistance <= stateMachine.DetectionRange)
            {
                stateMachine.ChangeState(EnemyStateType.Chase);
                return;
            }

            //제자리 대기 타입이면 이동하지 않음
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
            if (isWander)
            {
                //배회 목적지에 아직 도착하지 않았으면 계속 이동
                if (!stateMachine.EnemyMovement.HasReachedDestination)
                {
                    return;
                }

                //목적지에 도착하면 이동 정지
                stateMachine.EnemyMovement.Stop();

                //배회 종료 후 다시 대기 시작
                isWander = false;
                waitTimer = 0.0f;

                return;
            }

            waitTimer += Time.deltaTime;

            //설정된 대기 시간이 지나지 않았으면 계속 대기
            if (waitTimer < stateMachine.WanderWaitTime)
            {
                return;
            }

            //시작 위치 주변에서 이동 가능한 랜덤 위치 탐색
            if (stateMachine.EnemyMovement.TryGetRandomPoint(stateMachine.SpawnPosition, stateMachine.WanderRadius, out Vector3 destination))
            {
                //랜덤위치까지 걷기
                stateMachine.EnemyMovement.MoveTo(destination, stateMachine.EnemyMovement.WalkSpeed, 0.1f);
                isWander = true;
            }

            waitTimer = 0.0f;
        }
    }
}