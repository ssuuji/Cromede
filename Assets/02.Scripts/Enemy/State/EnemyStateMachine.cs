using System.Collections.Generic;
using UnityEngine;

namespace Cromede.Enemy.State
{
    public class EnemyStateMachine : MonoBehaviour
    {
        public enum EnemyStateType
        {
            Dormant,
            Wake,
            Idle,
            Chase,
            Attack,
            Hit,
            Return,
            Die
        }

        public enum EnemyStartType
        {
            Normal,
            Dormant
        }

        public enum EnemyIdleType
        {
            Stay,
            Wander
        }

        [Header("타겟")]
        [SerializeField] private Transform target;

        [Header("시작")]
        [SerializeField] private EnemyStartType startType = EnemyStartType.Normal;

        [Header("감지")]
        [SerializeField] private float detectionRange = 6.0f;
        [SerializeField] private float returnRange = 20.0f;

        [Header("비전투")]
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

        public float SpawnDistance => Vector3.Distance(transform.position, spawnPosition);

        private void Awake()
        {
            enemyMovement = GetComponent<EnemyMovement>();
            enemyAnimation = GetComponent<EnemyAnimation>();
            enemyAttack = GetComponent<EnemyAttack>();
            enemyHealth = GetComponent<EnemyHealth>();
            enemyCollider = GetComponent<Collider>();

            spawnPosition = transform.position;

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
            enemyHealth.OnDamaged += Damaged;
            enemyHealth.OnDied += Die;

            enemyAnimation.OnAnimationEnd += AnimationEnd;
        }

        private void OnDisable()
        {
            enemyHealth.OnDamaged -= Damaged;
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

        private void Damaged()
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

        private void Die()
        {
            ChangeState(EnemyStateType.Die);
        }

        private void AnimationEnd()
        {
            currentState?.OnAnimationEnd();
        }

        public void SetCollider(bool isEnabled)
        {
            if (enemyCollider != null)
            {
                enemyCollider.enabled = isEnabled;
            }
        }
    }
}