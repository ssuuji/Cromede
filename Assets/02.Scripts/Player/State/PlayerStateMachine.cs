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
        [SerializeField] private PlayerTargeting playerTargeting; //타겟팅 (카메라에 붙어있음)


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
        }

        private void OnDisable()
        {
            playerHealth.OnHit -= Hit;
            playerHealth.OnDied -= Die;
            playerHealth.OnRebirth -= Rebirth;
        }

        //private void Update()
        //{
        //    currentState?.Update();
        //}

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
        }

        //부활
        private void Rebirth()
        {
            ChangeState(PlayerStateType.Rebirth);
        }

        //테스트
        private void Update()
        {
            if (Keyboard.current.jKey.wasPressedThisFrame)
            {
                ChangeState(PlayerStateType.Stun);
            }

            if (Keyboard.current.kKey.wasPressedThisFrame)
            {
                ChangeState(PlayerStateType.KnockDown);
            }

            currentState?.Update();
        }
    }
}

