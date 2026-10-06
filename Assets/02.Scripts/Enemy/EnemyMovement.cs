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
        private float chaseStoppingDistance;

        public float WalkSpeed => walkSpeed;
        public float RunSpeed => runSpeed;
        public float ChaseStoppingDistance => chaseStoppingDistance;

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

        public bool HasReachedDestination
        {
            get
            {
                if (!agent.enabled || !agent.isOnNavMesh || agent.pathPending)
                {
                    return false;
                }

                if (agent.remainingDistance > agent.stoppingDistance + 0.05f)
                {
                    return false;
                }

                return !agent.hasPath || agent.velocity.sqrMagnitude <= 0.01f;
            }
        }

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            chaseStoppingDistance = agent.stoppingDistance;
        }

        public void MoveTo(Vector3 destination, float speed, float stoppingDistance)
        {
            if (!agent.enabled || !agent.isOnNavMesh)
            {
                return;
            }

            agent.isStopped = false;
            agent.speed = speed;
            agent.stoppingDistance = stoppingDistance;
            agent.SetDestination(destination);
        }

        public void Stop()
        {
            if (!agent.enabled || !agent.isOnNavMesh)
            {
                return;
            }

            agent.isStopped = true;
            agent.ResetPath();
        }

        public void FaceTarget(Vector3 targetPosition)
        {
            Vector3 targetDir = targetPosition - transform.position;
            targetDir.y = 0.0f;

            if (targetDir.sqrMagnitude <= 0.001f)
            {
                return;
            }

            transform.rotation = Quaternion.LookRotation(targetDir);
        }

        public bool TryGetRandomPoint(Vector3 center, float radius, out Vector3 point)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector3 randomPosition = center + Random.insideUnitSphere * radius;
                randomPosition.y = center.y;

                if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, radius, agent.areaMask))
                {
                    point = hit.position;
                    return true;
                }
            }

            point = center;
            return false;
        }
    }
}