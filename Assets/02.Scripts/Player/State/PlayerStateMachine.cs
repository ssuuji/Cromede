using Cromede.Input;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cromede.Player.State
{
    public class PlayerStateMachine : MonoBehaviour
    {
        public enum PlayerStateType
        {
            Move,
            Jump,
            Dodge,
            Sprint,
            Attack,
            Stun,
            KnockDown,
            Die,
            Rebirth,
            Interact
        }

        private Dictionary<PlayerStateType, PlayerState> states;
        private PlayerState currentState;      //현재 플레이어 상태 

        private PlayerInputSystem playerInput;   //인풋시스템
        private PlayerMovement playerMovement;   //이동
        private PlayerAttack playerAttack;       //공격
        private PlayerInteract playerInteract;   //상호작용
        private PlayerAnimation playerAnimation; //애니메이션
        private PlayerHealth playerHealth;
        [Header("카메라")]
        [SerializeField] private PlayerTargeting playerTargeting; //타겟팅 (카메라에 붙어있음)

        [Header("전투")]
        [SerializeField] private float cIdleDuration = 3.0f;

        private float cIdleTimer;
        private bool isCIdleTimer;
        private bool isCombat;

        public bool IsCombat => isCombat;


        public PlayerInputSystem PlayerInput => playerInput;
        public PlayerMovement PlayerMovement => playerMovement;
        public PlayerAttack PlayerAttack => playerAttack;
        public PlayerInteract PlayerInteract => playerInteract;
        public PlayerAnimation PlayerAnimation => playerAnimation;
        public PlayerTargeting PlayerTargeting => playerTargeting;


        private void Awake()
        {
            playerInput = GetComponent<PlayerInputSystem>();
            playerMovement = GetComponent<PlayerMovement>();
            playerAttack = GetComponent<PlayerAttack>();
            playerInteract = GetComponent<PlayerInteract>();
            playerHealth = GetComponent<PlayerHealth>();
            playerAnimation = GetComponentInChildren<PlayerAnimation>();

            //아래애들을 딕셔너리에 등록
            states = new Dictionary<PlayerStateType, PlayerState>()
            {
                { PlayerStateType.Move, new PlayerMoveState(this) },
                { PlayerStateType.Jump, new PlayerJumpState(this) },
                { PlayerStateType.Dodge, new PlayerDodgeState(this) },
                { PlayerStateType.Sprint, new PlayerSprintState(this) },
                { PlayerStateType.Attack, new PlayerAttackState(this) },
                { PlayerStateType.Stun, new PlayerStunState(this) },
                { PlayerStateType.KnockDown, new PlayerKnockDownState(this) },
                { PlayerStateType.Die, new PlayerDieState(this) },
                { PlayerStateType.Rebirth, new PlayerRebirthState(this) },
                { PlayerStateType.Interact, new PlayerInteractState(this) }
            };
        }
        private void Start()
        {
            ChangeState(PlayerStateType.Move); //처음은 이동상태로 설정
        }

        private void OnEnable()
        {
            playerHealth.OnHit += Hit;
            playerHealth.OnDied += Die;
            playerHealth.OnRebirth += Rebirth;

            playerAnimation.OnAnimationEnd += AnimationEnd;
        }

        private void OnDisable()
        {
            playerHealth.OnHit -= Hit;
            playerHealth.OnDied -= Die;
            playerHealth.OnRebirth -= Rebirth;

            playerAnimation.OnAnimationEnd -= AnimationEnd;
        }

        private void Update()
        {
            if (isCIdleTimer)
            {
                cIdleTimer += Time.deltaTime;

                if (cIdleTimer >= cIdleDuration)
                {
                    ExitCombat();
                }
            }

            if (currentState is not PlayerDieState && currentState is not PlayerRebirthState)
            {
                if (Keyboard.current.jKey.wasPressedThisFrame)
                {
                    ChangeState(PlayerStateType.Stun);
                }

                if (Keyboard.current.kKey.wasPressedThisFrame)
                {
                    ChangeState(PlayerStateType.KnockDown);
                }
            }

            currentState?.Update();
        }

        //애니메이션 종료
        private void AnimationEnd()
        {
            currentState?.OnAnimationEnd();
        }

        //상태 전환
        public void ChangeState(PlayerStateType nextStateType)
        {
            currentState?.Exit();

            currentState = states[nextStateType];

            currentState.Enter();
        }

        //점프 상태전환
        public void ChangeJumpState(bool isSprintJump)
        {
            PlayerJumpState jumpState = (PlayerJumpState)states[PlayerStateType.Jump];
            jumpState.SetSprintJump(isSprintJump);

            ChangeState(PlayerStateType.Jump);
        }

        //전투 상태 진입
        public void EnterCombat()
        {
            isCombat = true;
            cIdleTimer = 0.0f;
            isCIdleTimer = false;

            playerAnimation.SetCombat(true);
        }

        //전투 상태 종료
        public void ExitCombat()
        {
            isCombat = false;
            cIdleTimer = 0.0f;
            isCIdleTimer = false;

            playerAnimation.SetCombat(false);
        }

        //전투 대기 시간 시작
        public void StartCIdleTimer()
        {
            cIdleTimer = 0.0f;
            isCIdleTimer = true;
        }

        //피격
        private void Hit(Vector3 hitPosition)
        {
            if (currentState is not PlayerMoveState) return;

            Vector3 hitDir = hitPosition - transform.position;
            hitDir.y = 0.0f;
            hitDir.Normalize();

            Vector3 localHitDir = transform.InverseTransformDirection(hitDir);
            playerAnimation.SetHit(localHitDir.x, localHitDir.z);
        }

        //사망
        private void Die()
        {
            ChangeState(PlayerStateType.Die);
            ExitCombat();
        }

        //부활
        private void Rebirth()
        {
            ChangeState(PlayerStateType.Rebirth);
        }
    }
}

