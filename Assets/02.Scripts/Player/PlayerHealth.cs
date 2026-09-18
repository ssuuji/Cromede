using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cromede.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;
        private int currentHealth;
        private Coroutine burnCoroutine;

        public event Action OnDamaged;
        
        private void Awake()
        {
            currentHealth = maxHealth;
        }

        //피격
        public void TakeDamage(int damage)
        {
            currentHealth -= damage;

            Debug.Log($"{damage} : 플레이어 체력 {currentHealth}/{maxHealth}");
            if (currentHealth <= 0)
            {
                Die();
            }

            OnDamaged?.Invoke();
        }

        //사망
        private void Die()
        {
            currentHealth = 0;

            Debug.Log("플레이어 사망");
        }

        //화상데미지
        public void Burn(int damage, float duration, float damageInterval)
        {
            if (burnCoroutine != null)
            {
                StopCoroutine(burnCoroutine);
            }

            burnCoroutine = StartCoroutine(BurnCoroutine(damage, duration, damageInterval));
        }

        IEnumerator BurnCoroutine(int damage, float duration, float damageInterval)
        {
            float burnTimer = 0.0f;

            while (burnTimer < duration)
            {
                yield return new WaitForSeconds(damageInterval);

                TakeDamage(damage);

                burnTimer += damageInterval;
            }

            burnCoroutine = null;
        }

        //테스트
        private void Update()
        {
            if (Keyboard.current.hKey.wasPressedThisFrame)
            {
                TakeDamage(10);
            }

            if (Keyboard.current.lKey.wasPressedThisFrame)
            {
                Burn(5, 5.0f, 1.0f);
            }
        }
    }
}

