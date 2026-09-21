using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace Cromede.Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float moveDirDampTime = 0.1f;

        private static readonly int MoveMagnitudeHash = Animator.StringToHash("MoveMagnitude");
        private static readonly int MoveXHash = Animator.StringToHash("MoveX");
        private static readonly int MoveYHash = Animator.StringToHash("MoveY");
        private static readonly int DodgeHash = Animator.StringToHash("Dodge");
        private static readonly int DodgeXHash = Animator.StringToHash("DodgeX");
        private static readonly int DodgeYHash = Animator.StringToHash("DodgeY");

        private PlayerMovement playerMovement;
        private bool useRootMotion;

        private void Awake()
        {
            playerMovement = GetComponentInParent<PlayerMovement>();
        }

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
    }
}