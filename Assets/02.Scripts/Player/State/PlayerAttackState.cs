using UnityEngine;
using static Cromede.Player.State.PlayerStateMachine;

namespace Cromede.Player.State
{
    public class PlayerAttackState : PlayerState
    {
        public PlayerAttackState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        private float attackTimer;
        private int comboIndex;
        private bool nextAttack;

        public override void Enter()
        {
            comboIndex = 0;
            StartAttack();
        }

        public override void Update()
        {
            //공격하면서 움직일 수 있어야함
            stateMachine.PlayerMovement.Move(false);

            attackTimer += Time.deltaTime;

            //공격 중 다시 공격클릭 하면 다음 콤보 예약
            if (stateMachine.PlayerInput.IsAttack)
            {
                nextAttack = true;
            }

            if (attackTimer >= stateMachine.PlayerAttack.AttackDuration)
            {
                if (nextAttack && comboIndex < 3) // 콤보 4개라서,,
                {
                    comboIndex++;
                    StartAttack();
                    return;
                }
                stateMachine.ChangeState(PlayerStateType.Move);
            }
        }

        //공격
        private void StartAttack()
        {
            attackTimer = 0.0f;
            nextAttack = false;

            stateMachine.PlayerAttack.Attack();

            Debug.Log($"comboIndex : {comboIndex}");
        }
    }

}
