using UnityEngine;
using UnityEngine.AI;

namespace Cromede.Enemy
{
    public class EnemyMovement : MonoBehaviour
    {
        [Header("이동")]
        [SerializeField] private float walkSpeed = 1.5f;
        [SerializeField] private float runSpeed = 3.5f;

        private NavMeshAgent agent;
        private float chaseStoppingDistance; //추적 시 플레이어와 유지할 거리 (몹에 달려있는 NavMeshAgent에서 설정가능)

        public float WalkSpeed => walkSpeed;
        public float RunSpeed => runSpeed;
        public float ChaseStoppingDistance => chaseStoppingDistance;

        //현재 몬스터의 실제 이동 속도
        public float MoveMagnitude
        {
            get
            {
                if (!agent.enabled || !agent.isOnNavMesh)
                {
                    return 0.0f;
                }

                return agent.velocity.magnitude;
            }
        }

        //설정한 목적지에 도착했는지 확인
        public bool HasReachedDestination
        {
            get
            {
                //NavMeshAgent를 사용할 수 없거나 경로 계산 중이면 도착하지 않은 상태
                if (!agent.enabled || !agent.isOnNavMesh || agent.pathPending)
                {
                    return false;
                }

                //목적지까지 아직 이동할 거리가 남아있으면 도착하지 않은 상태
                if (agent.remainingDistance > agent.stoppingDistance + 0.05f) //0.05 도착판정 허용오차
                {
                    return false;
                }

                //경로가 끝났거나 이동 속도가 거의 0이면 도착
                return !agent.hasPath || agent.velocity.sqrMagnitude <= 0.01f;
            }
        }

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();

            chaseStoppingDistance = agent.stoppingDistance;
        }

        //목적지까지 이동
        public void MoveTo(Vector3 destination, float speed, float stoppingDistance)
        {
            if (!agent.enabled || !agent.isOnNavMesh)
            {
                return;
            }

            agent.isStopped = false;
            agent.speed = speed;
            agent.stoppingDistance = stoppingDistance;
            agent.SetDestination(destination); //목적지 설정
        }

        //이동 멈추기
        public void Stop()
        {
            if (!agent.enabled || !agent.isOnNavMesh)
            {
                return;
            }

            agent.isStopped = true;
            agent.ResetPath();
        }

        //지정한 위치 바라보기
        public void FaceTarget(Vector3 targetPosition)
        {
            //몬스터에서 타겟으로 향하는 방향 계산
            Vector3 targetDir = targetPosition - transform.position;
            targetDir.y = 0.0f;

            if (targetDir.sqrMagnitude <= 0.001f)
            {
                return;
            }

            transform.rotation = Quaternion.LookRotation(targetDir);
        }

        //시작 위치 주변에서 이동 가능한 랜덤 위치 찾기
        public bool TryGetRandomPoint(Vector3 center, float radius, out Vector3 point)
        {
            //최대 10번까지 랜덤 위치 탐색
            for (int i = 0; i < 10; i++)
            {
                Vector3 randomPosition = center + Random.insideUnitSphere * radius;
                randomPosition.y = center.y;

                //랜덤 위치 주변에서 실제 NavMesh 위의 이동 가능한 위치 찾기
                if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, radius, agent.areaMask))
                {
                    point = hit.position;
                    return true;
                }
            }

            //이동 가능한 위치를 찾지 못하면 시작 위치 반환
            point = center;
            return false;
        }
    }
}