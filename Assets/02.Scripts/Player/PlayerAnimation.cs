using UnityEngine;

namespace Cromede.Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float moveDirDampTime = 0.1f;
        [SerializeField] private float cIdleDuration = 3.0f;

        private float cIdleTimer;
        private bool isCIdleTimer;

        private static readonly int MoveMagnitudeHash = Animator.StringToHash("MoveMagnitude");
        private static readonly int MoveXHash = Animator.StringToHash("MoveX");
        private static readonly int MoveYHash = Animator.StringToHash("MoveY");
        private static readonly int DodgeHash = Animator.StringToHash("Dodge");
        private static readonly int DodgeXHash = Animator.StringToHash("DodgeX");
        private static readonly int DodgeYHash = Animator.StringToHash("DodgeY");
        private static readonly int SprintHash = Animator.StringToHash("Sprint");
        private static readonly int SprintStopHash = Animator.StringToHash("SprintStop");
        private static readonly int JumpHash = Animator.StringToHash("Jump");
        private static readonly int JumpLandHash = Animator.StringToHash("JumpLand");
        private static readonly int JumpLandRunHash = Animator.StringToHash("JumpLandRun");
        private static readonly int SprintJumpHash = Animator.StringToHash("SprintJump");
        private static readonly int AttackHash = Animator.StringToHash("Attack");
        private static readonly int IsCombatHash = Animator.StringToHash("IsCombat");
        private static readonly int HitHash = Animator.StringToHash("Hit");
        private static readonly int HitXHash = Animator.StringToHash("HitX");
        private static readonly int HitYHash = Animator.StringToHash("HitY");
        private static readonly int StunHash = Animator.StringToHash("Stun");
        private static readonly int KnockDownHash = Animator.StringToHash("KnockDown");
        private static readonly int KnockDownEndHash = Animator.StringToHash("KnockDownEnd");
        private static readonly int DieHash = Animator.StringToHash("Die");
        private static readonly int RebirthHash = Animator.StringToHash("Rebirth");
        private static readonly int InteractHash = Animator.StringToHash("Interact");
        private static readonly int InteractEndHash = Animator.StringToHash("InteractEnd");
        private static readonly int InteractCancelHash = Animator.StringToHash("InteractCancel");

        private PlayerMovement playerMovement;
        private bool useRootMotion;

        public bool IsCombat => animator.GetBool(IsCombatHash);

        private void Awake()
        {
            playerMovement = GetComponentInParent<PlayerMovement>();
        }

        private void Update()
        {
            if (!isCIdleTimer) return;

            cIdleTimer += Time.deltaTime;

            if (cIdleTimer >= cIdleDuration)
            {
                isCIdleTimer = false;
                SetCombat(false);
            }
        }

        #region 이동

        //움직임 여부
        public void SetMoveMagnitude(float moveMagnitude)
        {
            animator.SetFloat(MoveMagnitudeHash, moveMagnitude);
        }

        //이동
        public void SetMoveDir(float moveX, float moveY)
        {
            animator.SetFloat(MoveXHash, moveX, moveDirDampTime, Time.deltaTime);
            animator.SetFloat(MoveYHash, moveY, moveDirDampTime, Time.deltaTime);
        }

        #endregion

        #region 회피

        //회피
        public void SetDodge(float dodgeX, float dodgeY)
        {
            Vector2 dodgeAniDir = GetDodgeAniDir(dodgeX, dodgeY);

            animator.SetFloat(DodgeXHash, dodgeAniDir.x);
            animator.SetFloat(DodgeYHash, dodgeAniDir.y);
            animator.SetTrigger(DodgeHash);
        }

        private Vector2 GetDodgeAniDir(float dodgeX, float dodgeY)
        {
            float angle = Mathf.Atan2(dodgeX, dodgeY) * Mathf.Rad2Deg; //Atan2 : 두 값의 각도를 라디안 값으로 변환 , Rad2Deg(Radian To Degree) : 라디안값 -> 도 로 변환

            //음수처리
            if (angle < 0.0f)
            {
                angle += 360.0f;
            }

            //값 보정 후(22.5) 범위별 인덱스 구하기
            angle = (angle + 22.5f) % 360.0f;
            int dirIndex = Mathf.FloorToInt(angle / 45.0f); //소수점 삭제한 값 -> 방향 인덱스

            float dodgeAniX, dodgeAniY;
            switch (dirIndex)
            {
                case 0: // F
                    dodgeAniX = 0.0f;
                    dodgeAniY = 1.0f;
                    break;

                case 1: // FR
                    dodgeAniX = 1.0f;
                    dodgeAniY = 1.0f;
                    break;

                case 2: // R
                    dodgeAniX = 1.0f;
                    dodgeAniY = 0.0f;
                    break;

                case 3: // BR
                    dodgeAniX = 1.0f;
                    dodgeAniY = -1.0f;
                    break;

                case 4: // B
                    dodgeAniX = 0.0f;
                    dodgeAniY = -1.0f;
                    break;

                case 5: // BL
                    dodgeAniX = -1.0f;
                    dodgeAniY = -1.0f;
                    break;

                case 6: // L
                    dodgeAniX = -1.0f;
                    dodgeAniY = 0.0f;
                    break;

                case 7: // FL
                    dodgeAniX = -1.0f;
                    dodgeAniY = 1.0f;
                    break;

                default:
                    dodgeAniX = 0.0f;
                    dodgeAniY = 1.0f;
                    break;
            }

            return new Vector2(dodgeAniX, dodgeAniY);
        }
        #endregion

        #region 질주

        //질주
        public void SetSprint()
        {
            animator.SetTrigger(SprintHash);
        }

        public void SetSprintStop()
        {
            animator.SetTrigger(SprintStopHash);
        }

        #endregion

        #region 점프

        //점프
        public void SetJump()
        {
            animator.SetTrigger(JumpHash);
        }
        public void SetJumpLand()
        {
            animator.SetTrigger(JumpLandHash);
        }

        public void SetJumpLandRun()
        {
            animator.SetTrigger(JumpLandRunHash);
        }

        public void SetSprintJump()
        {
            animator.SetTrigger(SprintJumpHash);
        }

        #endregion

        #region 공격

        //전투 자세 설정
        public void SetCombat(bool isCombat)
        {
            animator.SetBool(IsCombatHash, isCombat);

            if (isCombat)
            {
                cIdleTimer = 0.0f;
                isCIdleTimer = false;
            }
        }

        //공격
        public void SetAttack()
        {
            animator.SetTrigger(AttackHash);
        }

        //전투 대기 시간 시작
        public void StartCIdleTimer()
        {
            cIdleTimer = 0.0f;
            isCIdleTimer = true;
        }
        #endregion

        #region 피격
        public void SetHit(float hitX, float hitY)
        {
            animator.SetFloat(HitXHash, hitX);
            animator.SetFloat(HitYHash, hitY);
            animator.SetTrigger(HitHash);
        }
        #endregion

        #region 스턴
        public void SetStun()
        {
            animator.SetTrigger(StunHash);
        }
        #endregion

        #region 넉다운
        public void SetKnockDown()
        {
            animator.SetTrigger(KnockDownHash);
        }

        public void SetKnockDownEnd()
        {
            animator.SetTrigger(KnockDownEndHash);
        }

        #endregion

        #region 사망
        public void SetDie()
        {
            animator.SetTrigger(DieHash);
        }
        #endregion

        #region 부활
        public void SetRebirth()
        {
            animator.SetTrigger(RebirthHash);
        }
        #endregion

        #region 상호작용
        public void SetInteract()
        {
            animator.SetTrigger(InteractHash);
        }

        public void SetInteractEnd()
        {
            animator.SetTrigger(InteractEndHash);
        }

        public void SetInteractCancel()
        {
            animator.SetTrigger(InteractCancelHash);
        }
        #endregion

        #region 루트모션

        //루트모션
        public void SetRootMotion(bool useRootMotion)
        {
            this.useRootMotion = useRootMotion;
        }
       
        private void OnAnimatorMove()
        {
            if (!useRootMotion) return;

            playerMovement.MoveRootMotion(animator.deltaPosition);
        }

        #endregion
    }
}