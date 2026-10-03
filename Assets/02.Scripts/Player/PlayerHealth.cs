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
        public event Action<Vector3> OnHit;
        public event Action OnDied;
        public event Action OnRebirth;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        //피격
        public void TakeDamage(int damage)
        {
            if (currentHealth <= 0) return;

            currentHealth -= damage;

            Debug.Log($"{damage} : 플레이어 체력 {currentHealth}/{maxHealth}");

            if (currentHealth <= 0)
            {
                Die();
                return;
            }

            OnDamaged?.Invoke();
        }

        //방향정보 피격 DmgF,B,L,R
        public void TakeDamage(int damage, Vector3 hitPosition)
        {
            if (currentHealth <= 0) return;

            TakeDamage(damage);

            if (currentHealth <= 0) return;

            OnHit?.Invoke(hitPosition);
        }

        //사망
        private void Die()
        {
            currentHealth = 0;

            if (burnCoroutine != null)
            {
                StopCoroutine(burnCoroutine);
                burnCoroutine = null;
            }

            OnDied?.Invoke();
        }

        //화상데미지
        public void Burn(int damage, float duration, float damageInterval)
        {
            if (currentHealth <= 0) return;

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

        //부활
        public void Rebirth()
        {
            if (currentHealth > 0) return;

            currentHealth = maxHealth;

            Debug.Log($"플레이어 부활 : {currentHealth}/{maxHealth}");

            OnRebirth?.Invoke();
        }


        //테스트
        private void Update()
        {
            //if (Keyboard.current.hKey.wasPressedThisFrame)
            //{
            //    TakeDamage(10);
            //}

            if (Keyboard.current.hKey.wasPressedThisFrame)
            {
                Vector3 hitPosition = transform.position + transform.forward * 2.0f; //F
                //Vector3 hitPosition = transform.position - transform.forward * 2.0f; //B
                //Vector3 hitPosition = transform.position + transform.right * 2.0f; //R
                //Vector3 hitPosition = transform.position - transform.right * 2.0f; //L
                TakeDamage(10, hitPosition);
            }

            if (Keyboard.current.lKey.wasPressedThisFrame)
            {
                Burn(5, 5.0f, 1.0f);
            }

            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                Rebirth();
            }
        }
    }
}

