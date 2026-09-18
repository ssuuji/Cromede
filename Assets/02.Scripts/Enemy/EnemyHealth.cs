using UnityEngine;

namespace Cromede.Enemy
{
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;
        private int currentHealth;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                Die();
            }
            Debug.Log($"{name} 데미지 : {damage} | 현재체력 : {currentHealth}/{maxHealth}");
        }

        private void Die()
        {
            currentHealth = 0;
            
            Debug.Log("사망");
        }
    }
}

