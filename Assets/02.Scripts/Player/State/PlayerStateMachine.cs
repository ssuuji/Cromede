using Cromede.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cromede.Player.State
{

    public class PlayerStateMachine : MonoBehaviour
    {
        private PlayerState currentState;     //현재 플레이어 상태 -> 딕셔너리? 상태이넘,,?
        private PlayerInputSystem playerInput; //인풋시스템

        private PlayerMovement playerMovement; //이동
        private PlayerAttack playerAttack;     //공격
        private PlayerInteract playerInteract; //상호작용

        private PlayerMoveState moveState;           //이동상태
        private PlayerDodgeState dodgeState;         //회피상태
        private PlayerSprintState sprintState;       //질주상태
        private PlayerAttackState attackState;       //공격상태
        private PlayerStunState stunState;           //스턴상태
        private PlayerKnockDownState knockDownState; //넉다운상태
        private PlayerInteractState interactState;   //상호작용상태


        public PlayerInputSystem PlayerInput => playerInput;

        public PlayerMovement PlayerMovement => playerMovement;
        public PlayerAttack PlayerAttack => playerAttack;
        public PlayerInteract PlayerInteract => playerInteract;

        public PlayerMoveState MoveState => moveState;
        public PlayerDodgeState DodgeState => dodgeState;
        public PlayerSprintState SprintState => sprintState;
        public PlayerAttackState AttackState => attackState;
        public PlayerStunState StunState => stunState;
        public PlayerKnockDownState KnockDownState => knockDownState;
        public PlayerInteractState InteractState => interactState;


        private void Awake()
        {
            playerInput = GetComponent<PlayerInputSystem>();

            playerMovement = GetComponent<PlayerMovement>();
            playerAttack = GetComponent<PlayerAttack>();
            playerInteract = GetComponent<PlayerInteract>();

            moveState = new PlayerMoveState(this);
            dodgeState = new PlayerDodgeState(this);
            sprintState = new PlayerSprintState(this);
            attackState = new PlayerAttackState(this);
            stunState = new PlayerStunState(this);
            knockDownState = new PlayerKnockDownState(this);
            interactState = new PlayerInteractState(this);
        }
        private void Start()
        {
            ChangeState(moveState); //처음은 이동상태로 설정
        }

        //private void Update()
        //{
        //    currentState?.Update();
        //}

        public void ChangeState(PlayerState nextState)
        {
            currentState?.Exit();

            currentState = nextState;

            currentState.Enter();
        }

        //테스트
        private void Update()
        {
            if (Keyboard.current.jKey.wasPressedThisFrame)
            {
                ChangeState(stunState);
            }

            if (Keyboard.current.kKey.wasPressedThisFrame)
            {
                ChangeState(knockDownState);
            }

            currentState?.Update();
        }
    }
}

