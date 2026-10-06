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

        private Transform currentTarget; //현재 선택된 타겟
        private PlayerInputSystem playerInput;

        public Transform CurrentTarget => currentTarget;

        private void Awake()
        {
            playerInput = player.GetComponent<PlayerInputSystem>();
        }

        private void Update()
        {
            //ESC 입력 시 타겟 해제
            if (playerInput.IsEsc)
            {
                currentTarget = null;
                return;
            }

            //타겟 변경 입력
            if (playerInput.IsTargetChange)
            {
                //범위 안의 몬스터 찾기
                Collider[] targets = GetTargets();

                //플레이어와 가까운 순서로 정렬
                Array.Sort(targets, (a, b) =>
                {
                    float enemyA = (a.transform.position - player.position).sqrMagnitude;
                    float enemyB = (b.transform.position - player.position).sqrMagnitude;

                    return enemyA.CompareTo(enemyB);
                });

                //다음 타겟 선택
                FindTarget(targets);

                if (currentTarget != null)
                {
                    Debug.Log($"currentTarget  {currentTarget.name}");
                }
            }
        }

        //타겟 범위 안의 몬스터 찾기
        private Collider[] GetTargets()
        {
            return Physics.OverlapSphere(player.position, targetRange, enemyLayer);
        }

        //현재 타겟을 기준으로 다음 타겟 선택
        private void FindTarget(Collider[] targets)
        {
            //범위 안에 몬스터가 없으면 타겟 해제
            if (targets.Length == 0)
            {
                currentTarget = null;
                return;
            }

            //현재 타겟이 없으면 가장 가까운 몬스터 선택
            if (currentTarget == null)
            {
                currentTarget = targets[0].transform;
                return;
            }

            int currentIndex = -1;

            //현재 타겟의 index 찾기
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].transform == currentTarget)
                {
                    currentIndex = i;
                    break;
                }
            }

            //기존 타겟이 범위에서 벗어났다면 가장 가까운 몬스터 선택
            if (currentIndex == -1)
            {
                currentTarget = targets[0].transform;
                return;
            }

            int nextIndex = currentIndex + 1;

            //마지막 타겟이면 다시 첫 번째 타겟으로
            if (nextIndex >= targets.Length)
            {
                nextIndex = 0;
            }

            currentTarget = targets[nextIndex].transform;
        }
    }
}