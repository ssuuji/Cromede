using Cromede.Input;
using UnityEngine;

namespace Cromede.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("카메라")]
        [SerializeField] private Transform mainCamera;

        [Header("이동")]
        [SerializeField] private float moveSpeed = 5f;       //이동속도
        [SerializeField] private float rotationSpeed = 720;  //초당 회전속도

        [Header("질주")]
        [SerializeField] private float sprintSpeed = 8.0f;

        [Header("점프")]
        [SerializeField] private float jumpForce = 10.5f;       //점프 힘
        [SerializeField] private float jumpGravity = 2.0f;      //점프 중 중력
        [SerializeField] private float airMoveSpeedRate = 0.5f; //제자리 점프 시 공중 이동속도 비율

        [Header("회피")]
        [SerializeField] private float dodgeMoveCancelTime = 0.35f;

        [Header("피격")]
        [SerializeField] private float damageSlowDuration = 1.0f;
        [SerializeField] private float damageSlowSpeedRate = 0.5f;

        private CharacterController characterController;
        private PlayerInputSystem playerInput;
        private PlayerHealth playerHealth;

        private float verticalVelocity;    //현재 플레이어의 Y축 속도
        private float damageSlowEndTime;   //피격 이동속도 감소가 끝나는 시간
        private Quaternion targetRotation; //캐릭터가 바라볼 목표 회전값
        private Vector3 jumpStartVelocity; //점프 시작 순간의 이동속도

        public bool IsGrounded => characterController.isGrounded;
        public float DodgeMoveCancelTime => dodgeMoveCancelTime;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            playerInput = GetComponent<PlayerInputSystem>();
            playerHealth = GetComponent<PlayerHealth>();

            //현재 방향을 초기 회전 목표로 설정
            targetRotation = transform.rotation;
        }

        private void OnEnable()
        {
            //피격 시 이동속도 감소
            playerHealth.OnDamaged += DamageSlow;
        }

        private void OnDisable()
        {
            playerHealth.OnDamaged -= DamageSlow;
        }

        #region 이동

        //일반 이동
        public void Move(bool rotateCharacter, Transform target = null)
        {
            MoveCharacter(moveSpeed, rotateCharacter, target);
        }

        //공통 이동 처리
        private void MoveCharacter(float speed, bool rotateCharacter, Transform target, float gravityMultiplier = 1.0f)
        {
            //중력 계산
            Gravity(gravityMultiplier);

            //카메라 기준 이동 방향 계산
            Vector3 moveDir = GetMoveDir();

            //캐릭터 회전이 필요한 경우
            if (rotateCharacter)
            {
                Vector3 rotateDir = moveDir;

                //타겟이 있으면 이동 방향이 아닌 타겟 방향 바라보기
                if (target != null)
                {
                    rotateDir = target.position - transform.position;
                    rotateDir.y = 0.0f;
                }

                Rotation(rotateDir);
            }

            //피격 감속 시간이 남아있으면 이동속도 감소
            float speedRate = Time.time < damageSlowEndTime ? damageSlowSpeedRate : 1.0f;

            //수평 이동속도 계산
            Vector3 velocity = moveDir * speed * speedRate;

            //수직 이동속도 적용
            velocity.y = verticalVelocity;

            //수평 + 수직 이동 적용
            characterController.Move(velocity * Time.deltaTime);
        }

        //현재 입력을 카메라 기준 월드 이동방향으로 변환
        public Vector3 GetMoveDir()
        {
            Vector2 moveInput = playerInput.MoveAction; //WASD 입력값
            Vector3 cameraForward = mainCamera.forward; //카메라 앞쪽 방향
            Vector3 cameraRight = mainCamera.right;     //카메라 오른쪽 방향

            //수평 이동만 사용하도록 Y축 제거
            cameraForward.y = 0.0f;
            cameraRight.y = 0.0f;

            //Forward와 Right 방향의 길이를 동일하게 맞춤
            cameraForward.Normalize();
            cameraRight.Normalize();

            //WS와 AD 입력 방향을 합쳐 이동 방향 계산
            Vector3 moveDir = cameraForward * moveInput.y + cameraRight * moveInput.x;

            //대각선 이동 시 속도가 빨라지지 않도록 정규화
            moveDir.Normalize();

            return moveDir;
        }

        //이동방향으로 캐릭터 회전
        private void Rotation(Vector3 moveDir)
        {
            //이동 방향이 있으면 새로운 회전 목표 저장
            if (moveDir.sqrMagnitude > 0.001f)
            {
                targetRotation = Quaternion.LookRotation(moveDir);
            }

            //목표 방향까지 부드럽게 회전
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        //현재 방향으로 회전 목표 초기화
        public void ResetTargetRotation()
        {
            targetRotation = transform.rotation;
        }

        #endregion

        #region 중력

        //중력 계산
        private void Gravity(float gravityMultiplier = 1.0f)
        {
            //지면에 있을 때 약한 아래 방향 힘을 유지해 Grounded 판정을 안정적으로 유지
            if (characterController.isGrounded && verticalVelocity < 0.0f)
            {
                verticalVelocity = -2.0f;
            }

            verticalVelocity += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        }

        //수직 이동만 처리
        public void VerticalMove()
        {
            Gravity();

            Vector3 velocity = Vector3.up * verticalVelocity;
            characterController.Move(velocity * Time.deltaTime);
        }

        #endregion

        #region 점프

        //점프 시작
        public void Jump()
        {
            if (!characterController.isGrounded) return;

            //점프 직전의 수평 이동속도 저장
            jumpStartVelocity = characterController.velocity;
            jumpStartVelocity.y = 0.0f;

            //Y축에 점프 힘 적용
            verticalVelocity = jumpForce;
        }

        //점프 중 이동
        public void JumpMove(bool rotateCharacter, Transform target = null)
        {
            //이동 중 점프면 기존 속도를 사용하고 제자리 점프면 기본 이동속도의 일정 비율 사용
            float jumpMoveSpeed = jumpStartVelocity.sqrMagnitude > 0.001f ? jumpStartVelocity.magnitude : moveSpeed * airMoveSpeedRate;

            MoveCharacter(jumpMoveSpeed, rotateCharacter, target, jumpGravity);
        }

        #endregion

        #region 질주

        //질주
        public void Sprint()
        {
            MoveCharacter(sprintSpeed, true, null);
        }

        #endregion

        #region 피격

        //피격 시 일정 시간 이동속도 감소
        private void DamageSlow()
        {
            //현재 시간에 지속시간을 더해 감속이 끝나는 시간 저장
            damageSlowEndTime = Time.time + damageSlowDuration;
        }

        #endregion

        #region 루트모션

        //애니메이션의 Root Motion 이동값 적용
        public void MoveRootMotion(Vector3 deltaPos)
        {
            //Y축은 기존 중력 사용
            Gravity();

            deltaPos.y = verticalVelocity * Time.deltaTime;
            characterController.Move(deltaPos);
        }

        #endregion
    }
}