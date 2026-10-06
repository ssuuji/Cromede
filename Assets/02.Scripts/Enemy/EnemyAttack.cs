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

        private float nextAttackTime; //다음 공격 가능 시간

        public int AttackCount => attackCount;
        public float AttackRange => attackRange;
        public float ComboInterval => comboInterval;
        public bool CanAttack => Time.time >= nextAttackTime;

        public void Attack()
        {
            //몬스터가 바라보는 방향
            Vector3 attackDir = transform.forward;
            attackDir.y = 0.0f;
            attackDir.Normalize();

            //공격 범위 안의 플레이어 찾기
            Collider[] targets = Physics.OverlapSphere(transform.position, attackRange, playerLayer);

            foreach (Collider target in targets)
            {
                //몬스터에서 플레이어로 향하는 방향
                Vector3 targetDir = target.transform.position - transform.position;
                targetDir.y = 0.0f;

                //몬스터 정면과 플레이어 방향 사이의 각도 계산
                float angle = Vector3.Angle(attackDir, targetDir);

                //공격 각도 안에 플레이어가 있을 경우
                if (angle <= attackAngle)
                {
                    PlayerHealth playerHealth = target.GetComponentInParent<PlayerHealth>();

                    if (playerHealth != null)
                    {
                        //플레이어에게 데미지 전달
                        playerHealth.TakeDamage(attackDamage, transform.position);
                    }
                }
            }
        }

        public void StartCooldown()
        {
            //현재 시간에 쿨타임을 더해 다음 공격 가능 시간 설정
            nextAttackTime = Time.time + attackCooldown;
        }
    }
}