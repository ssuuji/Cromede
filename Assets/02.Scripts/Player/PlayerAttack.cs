using Cromede.Enemy;
using UnityEngine;

namespace Cromede.Player
{
    public class PlayerAttack : MonoBehaviour
    {
        [Header("공격")]
        [SerializeField] private int attackDamage = 10;

        [Header("공격범위")]
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float attackAngle = 45.0f;
        [SerializeField] private LayerMask enemyLayer;

        [Header("콤보")]
        [SerializeField] private float comboInterval = 0.5f;

        public float ComboInterval => comboInterval;

        public void Attack()
        {
            //플레이어가 바라보는 방향
            Vector3 attackDir = transform.forward;
            attackDir.y = 0.0f;
            attackDir.Normalize();

            //공격 범위 안의 몬스터 찾기
            Collider[] targets = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

            foreach (Collider target in targets)
            {
                //플레이어에서 몬스터로 향하는 방향
                Vector3 targetDir = target.transform.position - transform.position;
                targetDir.y = 0.0f;

                //플레이어 정면과 몬스터 방향 사이의 각도 계산
                float angle = Vector3.Angle(attackDir, targetDir);

                //공격 각도 안에 몬스터가 있을 경우
                if (angle <= attackAngle)
                {
                    EnemyHealth enemyHealth = target.GetComponent<EnemyHealth>();

                    if (enemyHealth != null)
                    {
                        //몬스터에게 데미지 전달
                        enemyHealth.TakeDamage(attackDamage);
                    }
                }
            }
        }
    }
}