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
            Move,       //이동
            Jump,       //점프
            Dodge,      //회피
            Sprint,     //달리기
            Attack,     //공격
            Stun,       //기절
            KnockDown,  //넉다운
            Die,        //사망
            Rebirth,    //부활
            Interact    //상호작용
        }

        private Dictionary<PlayerStateType, PlayerState> states;
        private PlayerState currentState; //현재 플레이어 상태

        private PlayerInputSystem playerInput;   //입력
        private PlayerMovement playerMovement;   //이동
        private PlayerAttack playerAttack;       //공격
        private PlayerInteract playerInteract;   //상호작용
        private PlayerAnimation playerAnimation; //애니메이션
        private PlayerHealth playerHealth;       //체력

        [Header("카메라")]
        [SerializeField] private PlayerTargeting playerTargeting; //타겟팅 (카메라에 붙어있음)

        [Header("전투")]
        [SerializeField] private float cIdleDuration = 3.0f; //전투상태 유지 시간

        private float cIdleTimer;  //전투상태 타이머
        private bool isCIdleTimer; //전투상태 타이머 동작 여부
        private bool isCombat;     //전투상태 여부

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

            //플레이어 상태 등록
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
            //처음은 이동상태로 시작
            ChangeState(PlayerStateType.Move);
        }

        private void OnEnable()
        {
            //체력 이벤트 연결
            playerHealth.OnHit += Hit;
            playerHealth.OnDied += Die;
            playerHealth.OnRebirth += Rebirth;

            //애니메이션 종료 이벤트 연결
            playerAnimation.OnAnimationEnd += AnimationEnd;
        }

        private void OnDisable()
        {
            //체력 이벤트 해제
            playerHealth.OnHit -= Hit;
            playerHealth.OnDied -= Die;
            playerHealth.OnRebirth -= Rebirth;

            //애니메이션 종료 이벤트 해제
            playerAnimation.OnAnimationEnd -= AnimationEnd;
        }

        private void Update()
        {
            //전투상태 종료 타이머
            if (isCIdleTimer)
            {
                cIdleTimer += Time.deltaTime;

                if (cIdleTimer >= cIdleDuration)
                {
                    ExitCombat();
                }
            }

            //Stun / KnockDown 테스트용
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

            //현재 상태 Update 실행
            currentState?.Update();
        }

        //애니메이션 종료
        private void AnimationEnd()
        {
            currentState?.OnAnimationEnd();
        }

        //현재 상태를 종료하고 다음 상태로 전환
        public void ChangeState(PlayerStateType nextStateType)
        {
            currentState?.Exit();

            currentState = states[nextStateType];

            currentState.Enter();
        }

        //점프 종류 설정 후 점프 상태로 전환
        public void ChangeJumpState(bool isSprintJump)
        {
            PlayerJumpState jumpState = (PlayerJumpState)states[PlayerStateType.Jump];
            jumpState.SetSprintJump(isSprintJump);

            ChangeState(PlayerStateType.Jump);
        }

        //전투상태 진입
        public void EnterCombat()
        {
            isCombat = true;
            cIdleTimer = 0.0f;
            isCIdleTimer = false;

            playerAnimation.SetCombat(true);
        }

        //전투상태 종료
        public void ExitCombat()
        {
            isCombat = false;
            cIdleTimer = 0.0f;
            isCIdleTimer = false;

            playerAnimation.SetCombat(false);
        }

        //전투상태 종료 타이머 시작
        public void StartCIdleTimer()
        {
            cIdleTimer = 0.0f;
            isCIdleTimer = true;
        }

        //피격
        private void Hit(Vector3 hitPosition)
        {
            //이동상태에서만 일반 피격 애니메이션 재생
            if (currentState is not PlayerMoveState)
            {
                return;
            }

            //공격이 들어온 방향 계산
            Vector3 hitDir = hitPosition - transform.position;
            hitDir.y = 0.0f;
            hitDir.Normalize();

            //공격 방향을 플레이어 기준 방향으로 변환
            Vector3 localHitDir = transform.InverseTransformDirection(hitDir);

            //피격 방향에 맞는 애니메이션 재생
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