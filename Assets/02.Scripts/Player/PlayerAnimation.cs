using System;
using UnityEngine;

namespace Cromede.Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float moveDirDampTime = 0.1f; //이동 방향 애니메이션 보간 시간
        [SerializeField] private GameObject weapon;

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
        private bool useRootMotion;         //루트모션 사용 여부
        private Vector3 weaponPos;          //기존 무기 위치
        private Quaternion weaponRotation;  //기존 무기 회전값
        private bool weaponStun;            //스턴 전 무기 활성화 상태

        public event Action OnAnimationEnd;
        
        
        private void Awake()
        {
            playerMovement = GetComponentInParent<PlayerMovement>();

            //스턴 종료 후 원래 상태로 복구하기 위해 무기 위치와 회전값 저장
            weaponPos = weapon.transform.localPosition;
            weaponRotation = weapon.transform.localRotation;
        }
        

        #region 이동

        //움직임 여부
        public void SetMoveMagnitude(float moveMagnitude)
        {
            animator.SetFloat(MoveMagnitudeHash, moveMagnitude);
        }

        //이동 방향
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
            //입력 방향을 8방향 회피 애니메이션 값으로 변환
            Vector2 dodgeAniDir = GetDodgeAniDir(dodgeX, dodgeY);

            animator.SetFloat(DodgeXHash, dodgeAniDir.x);
            animator.SetFloat(DodgeYHash, dodgeAniDir.y);
            animator.SetTrigger(DodgeHash);
        }

        //회피 방향을 8방향 애니메이션 값으로 변환
        private Vector2 GetDodgeAniDir(float dodgeX, float dodgeY)
        {
            //입력 방향을 각도로 변환
            float angle = Mathf.Atan2(dodgeX, dodgeY) * Mathf.Rad2Deg;

            //음수각도를 0 ~ 360 범위로 변환
            if (angle < 0.0f)
            {
                angle += 360.0f;
            }

            //22.5도 보정 후 45도 단위로 나누어 8방향 인덱스 계산
            angle = (angle + 22.5f) % 360.0f;
            int dirIndex = Mathf.FloorToInt(angle / 45.0f);

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

        //질주 종료
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

        //제자리 착지
        public void SetJumpLand()
        {
            animator.SetTrigger(JumpLandHash);
        }

        //이동하면서 착지
        public void SetJumpLandRun()
        {
            animator.SetTrigger(JumpLandRunHash);
        }

        //질주 점프
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
            weapon.SetActive(isCombat);
        }

        //공격
        public void SetAttack()
        {
            animator.SetTrigger(AttackHash);
        }

        #endregion

        #region 피격

        //피격 방향 설정 후 피격 애니메이션 재생
        public void SetHit(float hitX, float hitY)
        {
            animator.SetFloat(HitXHash, hitX);
            animator.SetFloat(HitYHash, hitY);
            animator.SetTrigger(HitHash);
        }

        #endregion

        #region 스턴

        //스턴
        public void SetStun()
        {
            animator.SetTrigger(StunHash);
        }

        //스턴 애니메이션에 맞게 무기 위치와 회전 변경
        public void SetStunWeapon()
        {
            //스턴 전 무기 활성화 상태 저장
            weaponStun = weapon.activeSelf;

            weapon.SetActive(true);
            weapon.transform.localPosition = new Vector3(-0.00026f, -0.00166f, -0.00023f);
            weapon.transform.localRotation = Quaternion.Euler(180.0f, 0.0f, -104.01f);
        }

        //스턴 종료 후 무기 상태 원복
        public void ResetStunWeapon()
        {
            weapon.transform.localPosition = weaponPos;
            weapon.transform.localRotation = weaponRotation;
            weapon.SetActive(weaponStun);
        }

        #endregion

        #region 넉다운

        //넉다운
        public void SetKnockDown()
        {
            animator.SetTrigger(KnockDownHash);
        }

        //넉다운 종료
        public void SetKnockDownEnd()
        {
            animator.SetTrigger(KnockDownEndHash);
        }

        #endregion

        #region 사망

        //사망
        public void SetDie()
        {
            animator.SetTrigger(DieHash);
        }

        #endregion

        #region 부활

        //부활
        public void SetRebirth()
        {
            animator.SetTrigger(RebirthHash);
        }

        #endregion

        #region 상호작용

        //상호작용 시작
        public void SetInteract()
        {
            animator.SetTrigger(InteractHash);
        }

        //상호작용 종료
        public void SetInteractEnd()
        {
            animator.SetTrigger(InteractEndHash);
        }

        //상호작용 취소
        public void SetInteractCancel()
        {
            animator.SetTrigger(InteractCancelHash);
        }

        #endregion

        #region 루트모션

        //루트모션 사용 여부 설정
        public void SetRootMotion(bool useRootMotion)
        {
            this.useRootMotion = useRootMotion;
        }

        private void OnAnimatorMove()
        {
            //루트모션을 사용하지 않으면 처리하지 않음
            if (!useRootMotion) return;

            //애니메이션의 이동값을 실제 캐릭터 이동에 적용
            playerMovement.MoveRootMotion(animator.deltaPosition);
        }

        #endregion

        //애니메이션 종료
        public void AnimationEnd()
        {
            OnAnimationEnd?.Invoke();
        }
    }
}