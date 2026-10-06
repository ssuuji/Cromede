using System;
using UnityEngine;

namespace Cromede.Enemy
{
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;

        private int currentHealth;

        public event Action OnDamaged;
        public event Action OnDied;

        public bool IsDead => currentHealth <= 0;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(int damage)
        {
            if (currentHealth <= 0)
            {
                return;
            }

            currentHealth -= damage;

            if (currentHealth < 0)
            {
                currentHealth = 0;
            }

            Debug.Log($"{name} 데미지 : {damage} | 현재체력 : {currentHealth}/{maxHealth}");

            if (currentHealth <= 0)
            {
                OnDied?.Invoke();
                return;
            }

            OnDamaged?.Invoke();
        }
    }
}