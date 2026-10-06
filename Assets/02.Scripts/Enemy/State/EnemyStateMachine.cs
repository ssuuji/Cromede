using System.Collections.Generic;
using UnityEngine;

namespace Cromede.Enemy.State
{
    public class EnemyStateMachine : MonoBehaviour
    {
        public enum EnemyStateType
        {
            Dormant, //석상
            Wake,    //깨어남
            Idle,    //대기
            Chase,   //추적
            Attack,  //공격
            Hit,     //피격
            Return,  //복귀
            Die      //사망
        }

        public enum EnemyStartType
        {
            Normal,   //기본
            Dormant   //석상
        }

        public enum EnemyIdleType
        {
            Stay,     //제자리 대기
            Wander    //주변 돌아다님
        }

        [Header("타겟")]
        [SerializeField] private Transform target;

        [Header("시작상태")]
        [SerializeField] private EnemyStartType startType = EnemyStartType.Normal;

        [Header("감지")]
        [SerializeField] private float detectionRange = 6.0f;
        [SerializeField] private float returnRange = 20.0f;

        [Header("대기방식")]
        [SerializeField] private EnemyIdleType idleType = EnemyIdleType.Stay;
        [SerializeField] private float wanderRadius = 4.0f;
        [SerializeField] private float wanderWaitTime = 2.0f;


        private Dictionary<EnemyStateType, EnemyState> states;
        private EnemyState currentState;
        private EnemyMovement enemyMovement;
        private EnemyAnimation enemyAnimation;
        private EnemyAttack enemyAttack;
        private EnemyHealth enemyHealth;
        private Collider enemyCollider;
        private Vector3 spawnPosition;
        private bool isCombat;


        public Transform Target => target;
        public EnemyMovement EnemyMovement => enemyMovement;
        public EnemyAnimation EnemyAnimation => enemyAnimation;
        public EnemyAttack EnemyAttack => enemyAttack;
        public EnemyStartType StartType => startType;
        public EnemyIdleType IdleType => idleType;
        public Vector3 SpawnPosition => spawnPosition;
        public float DetectionRange => detectionRange;
        public float ReturnRange => returnRange;
        public float WanderRadius => wanderRadius;
        public float WanderWaitTime => wanderWaitTime;
        public bool HasTarget => target != null;

        //몬스터와 플레이어 사이 거리
        public float TargetDistance
        {
            get
            {
                if (target == null)
                {
                    return float.MaxValue;
                }

                return Vector3.Distance(transform.position, target.position);
            }
        }

        //몬스터와 시작 위치 사이 거리
        public float SpawnDistance => Vector3.Distance(transform.position, spawnPosition);


        private void Awake()
        {
            enemyMovement = GetComponent<EnemyMovement>();
            enemyAnimation = GetComponent<EnemyAnimation>();
            enemyAttack = GetComponent<EnemyAttack>();
            enemyHealth = GetComponent<EnemyHealth>();
            enemyCollider = GetComponent<Collider>();

            spawnPosition = transform.position; //스폰위치 저장

            states = new Dictionary<EnemyStateType, EnemyState>()
            {
                { EnemyStateType.Dormant, new EnemyDormantState(this) },
                { EnemyStateType.Wake, new EnemyWakeState(this) },
                { EnemyStateType.Idle, new EnemyIdleState(this) },
                { EnemyStateType.Chase, new EnemyChaseState(this) },
                { EnemyStateType.Attack, new EnemyAttackState(this) },
                { EnemyStateType.Hit, new EnemyHitState(this) },
                { EnemyStateType.Return, new EnemyReturnState(this) },
                { EnemyStateType.Die, new EnemyDieState(this) }
            };
        }

        private void Start()
        {
            if (startType == EnemyStartType.Dormant)
            {
                ChangeState(EnemyStateType.Dormant);
                return;
            }

            ChangeState(EnemyStateType.Idle);
        }

        private void OnEnable()
        {
            enemyHealth.OnDamaged += Hit;
            enemyHealth.OnDied += Die;

            enemyAnimation.OnAnimationEnd += AnimationEnd;
        }

        private void OnDisable()
        {
            enemyHealth.OnDamaged -= Hit;
            enemyHealth.OnDied -= Die;

            enemyAnimation.OnAnimationEnd -= AnimationEnd;
        }

        private void Update()
        {
            currentState?.Update();

            enemyAnimation.SetMoveMagnitude(enemyMovement.MoveMagnitude);
        }

        public void ChangeState(EnemyStateType nextStateType)
        {
            currentState?.Exit();

            currentState = states[nextStateType];

            currentState.Enter();
        }

        #region 전투

        public void EnterCombat()
        {
            if (isCombat)
            {
                return;
            }

            isCombat = true;
            enemyAnimation.SetCombat(true);
        }

        public void ExitCombat()
        {
            if (!isCombat)
            {
                return;
            }

            isCombat = false;
            enemyAnimation.SetCombat(false);
        }

        #endregion

        #region 피격
        private void Hit()
        {
            if (currentState is EnemyDieState)
            {
                return;
            }

            //복귀 중에는 다시 어그로를 잡지 않음
            if (currentState is EnemyReturnState)
            {
                return;
            }

            //이미 피격 중이면 다시 재생하지 않음
            if (currentState is EnemyHitState)
            {
                return;
            }

            ChangeState(EnemyStateType.Hit);
        }
        #endregion

        #region 사망
        private void Die()
        {
            ChangeState(EnemyStateType.Die);
        }
        #endregion

        private void AnimationEnd()
        {
            currentState?.OnAnimationEnd();
        }

        //몬스터 충돌 판정 활성화/비활성화
        public void SetCollider(bool isEnabled)
        {
            if (enemyCollider != null)
            {
                enemyCollider.enabled = isEnabled;
            }
        }
    }
}