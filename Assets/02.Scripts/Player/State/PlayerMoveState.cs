using UnityEngine;
using static Cromede.Player.State.PlayerStateMachine;

namespace Cromede.Player.State
{
    public class PlayerMoveState : PlayerState
    {
        public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Update()
        {
            //회피
            if (stateMachine.PlayerInput.IsSprint && stateMachine.PlayerMovement.IsGrounded)
            {
                stateMachine.ChangeState(PlayerStateType.Dodge);
                return;
            }

            //공격
            if (stateMachine.PlayerInput.IsAttack)
            {
                stateMachine.ChangeState(PlayerStateType.Attack);
                return;
            }

            //상호작용
            if (stateMachine.PlayerInput.IsInteract)
            {
                stateMachine.PlayerInteract.Interact();
                return;
            }

            //애니메이션
            stateMachine.PlayerAnimation.SetMoveMagnitude(stateMachine.PlayerInput.MoveAction.magnitude);

            Transform currentTarget = stateMachine.PlayerTargeting.CurrentTarget;

            if (currentTarget == null)
            {
                stateMachine.PlayerAnimation.SetMoveDir(0.0f, 1.0f);
            }
            else
            {
                Vector3 moveDir = stateMachine.PlayerMovement.GetMoveDir();
                Vector3 localMoveDir = stateMachine.transform.InverseTransformDirection(moveDir);
                stateMachine.PlayerAnimation.SetMoveDir(localMoveDir.x, localMoveDir.z);
            }

            stateMachine.PlayerMovement.Move(true, currentTarget);
        }
    }
}
