using UnityEngine;
using UnityEngine.InputSystem;

namespace Cromede.Input
{
    public class PlayerInputSystem : MonoBehaviour
    {
        private GameInputAction inputActions; //InputSystem_Actions에서 Generate C# Class로 자동생성한거 이번에 이거 써보는걸로
       
        private InputAction moveAction;            //WASD 이동
        private InputAction lookAction;            //마우스 이동
        private InputAction jumpAction;            //점프 Space
        private InputAction attackAction;          //기본공격 왼쪽마우스
        private InputAction secondaryAttackAction; //보조공격 오른쪽마우스
        private InputAction sprintAction;          //회피/스프린트 Shift
        private InputAction targetChangeAction;    //타겟 변경 Tab
        private InputAction interactAction;        //상호작용 F
        private InputAction cursorModeAction;      //마우스 커서 활성화 Alt

        public Vector2 MoveAction => moveAction.ReadValue<Vector2>();
        public Vector2 LookAction => lookAction.ReadValue<Vector2>();
        public bool IsJump => jumpAction.WasPressedThisFrame();
        public bool IsAttack => attackAction.WasPressedThisFrame();
        public bool IsSecondaryAttack => secondaryAttackAction.WasPressedThisFrame();
        public bool IsSprint => sprintAction.WasPressedThisFrame();
        public bool IsTargetChange => targetChangeAction.WasPressedThisFrame();
        public bool IsInteract => interactAction.WasPressedThisFrame();
        public bool IsCursorMode => cursorModeAction.IsPressed();

        private void Awake()
        {
            inputActions = new GameInputAction();
            moveAction = inputActions.Player.Move;
            lookAction = inputActions.Player.Look;
            jumpAction = inputActions.Player.Jump;
            attackAction = inputActions.Player.Attack;
            secondaryAttackAction = inputActions.Player.SecondaryAttack;
            sprintAction = inputActions.Player.Sprint;
            targetChangeAction = inputActions.Player.TargetChange;
            interactAction = inputActions.Player.Interact;
            cursorModeAction = inputActions.Player.CursorMode;
        }

        private void OnEnable()
        {
            inputActions.Player.Enable();
        }

        private void OnDisable()
        {
            inputActions.Player.Disable();
        }

        private void OnDestroy()
        {
            inputActions.Dispose();
        }
    }
}

