using UnityEngine;
using static Cromede.Player.State.PlayerStateMachine;

namespace Cromede.Player.State
{
    public class PlayerInteractState : PlayerState
    {
        public PlayerInteractState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        private float interactTimer;
        private bool isInteractEnd;

        public override void Enter()
        {
            interactTimer = 0.0f;
            isInteractEnd = false;

            stateMachine.PlayerInteract.Interact();
            stateMachine.PlayerAnimation.SetInteract();
        }

        public override void Update()
        {
            interactTimer += Time.deltaTime;

            //상호작용중 움직이면 상호작용 취소
            if (stateMachine.PlayerInput.MoveAction.sqrMagnitude > 0.001f)
            {
                stateMachine.PlayerAnimation.SetInteractCancel();
                stateMachine.ChangeState(PlayerStateType.Move);
                return;
            }

            //상호작용 종료
            if (!isInteractEnd && interactTimer >= 3.0f)
            {
                isInteractEnd = true;
                interactTimer = 0.0f;

                stateMachine.PlayerAnimation.SetInteractEnd();
                return;
            }

            //종료 애니메이션 재생 후 이동 상태로 복귀
            if (isInteractEnd && interactTimer >= 0.958f)
            {
                stateMachine.ChangeState(PlayerStateType.Move);
            }
        }
    }
}