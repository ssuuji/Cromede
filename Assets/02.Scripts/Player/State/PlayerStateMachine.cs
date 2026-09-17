using Cromede.Input;
using UnityEngine;

namespace Cromede.Player.State
{
    public class PlayerStateMachine : MonoBehaviour
    {
        private IPlayerState currentState;     //현재 플레이어 상태
        private PlayerInputSystem playerInput; //인풋시스템

        private PlayerMovement playerMovement; //이동

        private PlayerMoveState moveState;     //이동상태
        private PlayerDodgeState dodgeState;   //회피상태
        private PlayerSprintState sprintState; //질주상태


        public PlayerInputSystem PlayerInput => playerInput;
        public PlayerMovement PlayerMovement => playerMovement;
        public PlayerMoveState MoveState => moveState;
        public PlayerDodgeState DodgeState => dodgeState;
        public PlayerSprintState SprintState => sprintState;


        private void Awake()
        {
            playerInput = GetComponent<PlayerInputSystem>();

            playerMovement = GetComponent<PlayerMovement>();
            playerMovement.Init(playerInput);

            moveState = new PlayerMoveState(this);
            dodgeState = new PlayerDodgeState(this);
            sprintState = new PlayerSprintState(this);
        }
        private void Start()
        {
            ChangeState(moveState); //처음은 이동상태로 설정
        }

        private void Update()
        {
            currentState?.Update();
        }

        public void ChangeState(IPlayerState nextState)
        {
            currentState?.Exit();

            currentState = nextState;

            currentState.Enter();
        }
    }
}

