using System;
using UnityEngine;

namespace Cromede.Enemy
{
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;

        private int currentHealth; //현재 체력

        public event Action OnDamaged;
        public event Action OnDied;

        public bool IsDead => currentHealth <= 0;

        private void Awake()
        {
            //게임 시작 시 최대 체력으로 초기화
            currentHealth = maxHealth;
        }

        public void TakeDamage(int damage)
        {
            //이미 사망한 상태면 데미지 처리하지 않음
            if (currentHealth <= 0)
            {
                return;
            }

            //데미지만큼 체력 감소
            currentHealth -= damage;

            //체력이 0보다 내려가지 않도록 처리
            if (currentHealth < 0)
            {
                currentHealth = 0;
            }

            Debug.Log($"{name} 데미지 : {damage} | 현재체력 : {currentHealth}/{maxHealth}");

            //체력이 0이면 사망 이벤트 실행
            if (currentHealth <= 0)
            {
                OnDied?.Invoke();
                return;
            }

            //살아있으면 피격 이벤트 실행
            OnDamaged?.Invoke();
        }
    }
}