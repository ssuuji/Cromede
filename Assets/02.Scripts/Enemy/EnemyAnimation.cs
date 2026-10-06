using System;
using UnityEngine;

namespace Cromede.Enemy
{
    public class EnemyAnimation : MonoBehaviour
    {
        private Animator animator;

        private static readonly int MoveMagnitudeHash = Animator.StringToHash("MoveMagnitude");
        private static readonly int IsCombatHash = Animator.StringToHash("IsCombat");
        private static readonly int DormantHash = Animator.StringToHash("Dormant");
        private static readonly int WakeHash = Animator.StringToHash("Wake");
        private static readonly int AttackHash = Animator.StringToHash("Attack");
        private static readonly int HitHash = Animator.StringToHash("Hit");
        private static readonly int DieHash = Animator.StringToHash("Die");

        public event Action OnAnimationEnd;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void AnimationEnd()
        {
            OnAnimationEnd?.Invoke();
        }

        public void SetMoveMagnitude(float moveMagnitude)
        {
            animator.SetFloat(MoveMagnitudeHash, moveMagnitude);
        }

        public void SetCombat(bool isCombat)
        {
            animator.SetBool(IsCombatHash, isCombat);
        }

        public void SetDormant()
        {
            animator.SetTrigger(DormantHash);
        }

        public void SetWake()
        {
            animator.SetTrigger(WakeHash);
        }

        public void SetAttack()
        {
            animator.SetTrigger(AttackHash);
        }

        public void SetHit()
        {
            animator.SetTrigger(HitHash);
        }

        public void SetDie()
        {
            animator.SetTrigger(DieHash);
        }
    }
}