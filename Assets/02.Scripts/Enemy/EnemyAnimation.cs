using UnityEngine;

namespace Cromede.Enemy
{
    public class EnemyAnimation : MonoBehaviour
    {
        private Animator animator;

        private readonly int moveMagnitudeHash = Animator.StringToHash("MoveMagnitude");
        private readonly int isCombatHash = Animator.StringToHash("IsCombat");
        private readonly int attackHash = Animator.StringToHash("Attack");


        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void SetMoveMagnitude(float moveMagnitude)
        {
            animator.SetFloat(moveMagnitudeHash, moveMagnitude);
        }

        public void SetCombat(bool isCombat)
        {
            animator.SetBool(isCombatHash, isCombat);
        }

        public void PlayAttack()
        {
            animator.SetTrigger(attackHash);
        }
    }
}