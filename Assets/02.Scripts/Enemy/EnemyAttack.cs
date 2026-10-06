using Cromede.Player;
using UnityEngine;

namespace Cromede.Enemy
{
    public class EnemyAttack : MonoBehaviour
    {
        [Header("공격")]
        [SerializeField] private int attackDamage = 10;

        [Header("콤보")]
        [SerializeField] private int attackCount = 2;
        [SerializeField] private float comboInterval = 0.5f;
        [SerializeField] private float attackCooldown = 1.0f;

        [Header("공격범위")]
        [SerializeField] private float attackRange = 3.0f;
        [SerializeField] private float attackAngle = 45.0f;
        [SerializeField] private LayerMask playerLayer;

        private float nextAttackTime;

        public int AttackCount => attackCount;
        public float AttackRange => attackRange;
        public float ComboInterval => comboInterval;
        public bool CanAttack => Time.time >= nextAttackTime;

        public void Attack()
        {
            Vector3 attackDir = transform.forward;
            attackDir.y = 0.0f;
            attackDir.Normalize();

            Collider[] targets = Physics.OverlapSphere(transform.position, attackRange, playerLayer);

            foreach (Collider target in targets)
            {
                Vector3 targetDir = target.transform.position - transform.position;
                targetDir.y = 0.0f;

                float angle = Vector3.Angle(attackDir, targetDir);

                if (angle <= attackAngle)
                {
                    PlayerHealth playerHealth = target.GetComponentInParent<PlayerHealth>();

                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(attackDamage, transform.position);
                    }
                }
            }
        }

        public void StartCooldown()
        {
            nextAttackTime = Time.time + attackCooldown;
        }
    }
}