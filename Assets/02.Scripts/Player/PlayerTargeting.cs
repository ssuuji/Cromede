using Cromede.Input;
using System;
using UnityEngine;

namespace Cromede.Player
{
    public class PlayerTargeting : MonoBehaviour
    {
        [SerializeField] private Transform player;

        [Header("타겟")]
        [SerializeField] private float targetRange = 30.0f;
        [SerializeField] private LayerMask enemyLayer;

        private Transform currentTarget;
        private PlayerInputSystem playerInput;

        public Transform CurrentTarget => currentTarget;

        private void Awake()
        {
            playerInput = player.GetComponent<PlayerInputSystem>();
        }

        private void Update()
        {
            if (playerInput.IsEsc)
            {
                currentTarget = null;
                return;
            }
            if (playerInput.IsTargetChange)
            {
                Collider[] targets = GetTargets();

                Array.Sort(targets, (a, b) =>
                {
                    float enemyA = (a.transform.position - player.position).sqrMagnitude;
                    float enemyB = (b.transform.position - player.position).sqrMagnitude;

                    return enemyA.CompareTo(enemyB);
                });

                FindTarget(targets);

                if (currentTarget != null)
                {
                    Debug.Log($"currentTarget  {currentTarget.name}");
                }
            }
        }

        private Collider[] GetTargets()
        {
            return Physics.OverlapSphere(player.position, targetRange, enemyLayer);
        }

        private void FindTarget(Collider[] targets)
        {
            if (targets.Length == 0)
            {
                currentTarget = null;
                return;
            }

            if (currentTarget == null)
            {
                //타겟이 없다면 가장 가까운 타겟을 현재 타겟으로
                currentTarget = targets[0].transform;
                return;
            }

            int currentIndex = -1;

            //현재타겟의 index찾기
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].transform == currentTarget)
                {
                    currentIndex = i;
                    break;
                }
            }

            //기존 타겟이 없다면 가장 가까운 타겟으로 변경
            if (currentIndex == -1)
            {
                currentTarget = targets[0].transform;
                return;
            }

            int nextIndex = currentIndex + 1;

            //다음 index가 끝이라면 다시 0 부터
            if (nextIndex >= targets.Length)
            {
                nextIndex = 0;
            }

            currentTarget = targets[nextIndex].transform;
        }
    }
}
