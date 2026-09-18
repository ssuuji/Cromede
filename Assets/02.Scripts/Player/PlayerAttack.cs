using Cromede.Enemy;
using UnityEngine;

namespace Cromede.Player
{
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private Transform mainCamera;
        [Header("공격")]
        [SerializeField] private float attackDuration = 0.5f;
        [SerializeField] private int attackDamage = 10;

        [Header("공격범위")]
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float attackAngle = 45.0f;
        [SerializeField] private LayerMask enemyLayer;

        [Header("콤보")]
        [SerializeField] private int comboIndex;
        [SerializeField] private bool nextAttack;

        public float AttackDuration => attackDuration;

        public void Attack()
        {
            //카메라 방향으로 공격. 해당 방향으로 몸을 돌려 공격.
            Vector3 attackDir = mainCamera.forward;
            attackDir.y = 0.0f;
            attackDir.Normalize();
            transform.rotation = Quaternion.LookRotation(attackDir);


            Collider[] targets = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);
            foreach (Collider target in targets)
            {
                Vector3 targetDir = target.transform.position - transform.position;
                targetDir.y = 0.0f;

                float angle = Vector3.Angle(attackDir, targetDir);
                if (angle <= attackAngle)
                {
                    EnemyHealth enemyHealth = target.GetComponent<EnemyHealth>();

                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(attackDamage);
                    }
                }
            }
        }
    }

}
