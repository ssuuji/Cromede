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

        #region 이동

        //이동 속도값 설정
        public void SetMoveMagnitude(float moveMagnitude)
        {
            animator.SetFloat(MoveMagnitudeHash, moveMagnitude);
        }

        #endregion

        #region 전투

        //전투상태 설정
        public void SetCombat(bool isCombat)
        {
            animator.SetBool(IsCombatHash, isCombat);
        }

        #endregion

        #region 석상

        //석상상태 
        public void SetDormant()
        {
            animator.SetTrigger(DormantHash);
        }

        //깨어남 
        public void SetWake()
        {
            animator.SetTrigger(WakeHash);
        }

        #endregion

        #region 공격

        //공격
        public void SetAttack()
        {
            animator.SetTrigger(AttackHash);
        }

        #endregion

        #region 피격

        //피격 
        public void SetHit()
        {
            animator.SetTrigger(HitHash);
        }

        #endregion

        #region 사망

        //사망
        public void SetDie()
        {
            animator.SetTrigger(DieHash);
        }

        #endregion

        //애니메이션 종료
        public void AnimationEnd()
        {
            OnAnimationEnd?.Invoke();
        }
    }
}