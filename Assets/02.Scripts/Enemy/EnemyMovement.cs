using UnityEngine;
using UnityEngine.AI;

namespace Cromede.Enemy
{
    public class EnemyMovement : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float detectionRange = 6.0f;
        [SerializeField] private float returnRange = 20.0f;
        [SerializeField] private float attackRange = 3.0f;

        private NavMeshAgent agent;
        private EnemyAnimation enemyAnimation;

        private Vector3 spawnPosition;
        private float chaseStoppingDistance;
        private bool isAggro;
        private bool isReturning;
        private bool isAttacking;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            enemyAnimation = GetComponent<EnemyAnimation>();

            spawnPosition = transform.position;
            chaseStoppingDistance = agent.stoppingDistance;
        }

        private void Update()
        {
            if (target == null)
            {
                return;
            }

            if (isReturning)
            {
                ReturnToSpawn();
                return;
            }

            float targetDistance = Vector3.Distance(transform.position, target.position);

            //처음 플레이어 감지
            if (!isAggro)
            {
                if (targetDistance > detectionRange)
                {
                    enemyAnimation.SetMoveMagnitude(0.0f);
                    enemyAnimation.SetCombat(false);
                    return;
                }

                isAggro = true;
            }

            float spawnDistance = Vector3.Distance(transform.position, spawnPosition);

            //최대 추적 거리 초과
            if (spawnDistance > returnRange)
            {
                isAggro = false;
                isReturning = true;
                agent.stoppingDistance = 0.0f;
                return;
            }

            enemyAnimation.SetCombat(true);

            if (targetDistance <= attackRange)
            {
                agent.ResetPath();
                enemyAnimation.SetMoveMagnitude(0.0f);

                if (!isAttacking)
                {
                    isAttacking = true;
                    enemyAnimation.PlayAttack();
                }

                return;
            }

            agent.SetDestination(target.position);
            enemyAnimation.SetMoveMagnitude(agent.velocity.magnitude);
        }

        private void ReturnToSpawn()
        {
            enemyAnimation.SetCombat(false);
            agent.SetDestination(spawnPosition);
            enemyAnimation.SetMoveMagnitude(agent.velocity.magnitude);

            if (Vector3.Distance(transform.position, spawnPosition) > 0.2f)
            {
                return;
            }

            agent.ResetPath();
            agent.stoppingDistance = chaseStoppingDistance;

            isReturning = false;

            enemyAnimation.SetMoveMagnitude(0.0f);
            enemyAnimation.SetCombat(false);
        }

        public void AnimationEnd()
        {
            isAttacking = false;
        }
    }
}