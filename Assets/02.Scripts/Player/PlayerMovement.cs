using Cromede.Input;
using UnityEngine;

namespace Cromede.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("카메라")]
        [SerializeField] private Transform mainCamera;

        [Header("이동")]
        [SerializeField] private float moveSpeed = 5.0f;       //이동속도
        [SerializeField] private float rotationSpeed = 2000f;  //초당 회전 속도

        [Header("점프")]
        [SerializeField] private float jumpForce = 5.5f;       //점프 힘

        [Header("회피")]
        [SerializeField] private float dodgeSpeed = 25.0f;     //회피 속도
        [SerializeField] private float dodgeDuration = 0.2f;   //회피 유지시간

        [Header("질주")]
        [SerializeField] private float sprintSpeed = 10.0f;

        [Header("피격")]
        [SerializeField] private float damageSlowDuration = 1.0f;
        [SerializeField] private float damageSlowSpeedRate = 0.5f; 
        
        private CharacterController characterController;
        private PlayerInputSystem playerInput;
        private PlayerHealth playerHealth;

        private float verticalVelocity; //현재 플레이어의 Y축 속도
        private float damageSlowEndTime;


        public bool IsGrounded => characterController.isGrounded;
        public float DodgeDuration => dodgeDuration;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            playerInput = GetComponent<PlayerInputSystem>();
            playerHealth = GetComponent<PlayerHealth>();
        }

        private void OnEnable()
        {
            playerHealth.OnDamaged += DamageSlow;
        }

        private void OnDisable()
        {
            playerHealth.OnDamaged -= DamageSlow;
        }

        //이동
        public void Move(bool rotateCharacter)
        {
            MoveCharacter(moveSpeed, rotateCharacter);
        }

        //공통 이동(걷기,달리기)
        private void MoveCharacter(float speed, bool rotateCharacter)
        {
            Jump();
            Gravity();

            Vector3 moveDir = GetMoveDir();

            if (rotateCharacter)
            {
                Rotation(moveDir);
            }

            float speedRate = Time.time < damageSlowEndTime ? damageSlowSpeedRate : 1.0f;
            Vector3 velocity = moveDir * speed * speedRate;      //수평 이동속도
            velocity.y = verticalVelocity;                       //그리고 수직속도 합침
            characterController.Move(velocity * Time.deltaTime); //수평 + 수직 적용
        }

        //현재입력을 카메라기준 월드이동방향으로 전환
        public Vector3 GetMoveDir()
        {
            Vector2 moveInput = playerInput.MoveAction;//WASD 입력 값
            Vector3 cameraForward = mainCamera.forward; //카메라 앞쪽 방향
            Vector3 cameraRight = mainCamera.right;    //카메라 오른쪽 방향

            //y축 고정
            cameraForward.y = 0.0f; 
            cameraRight.y = 0.0f;

            //정규화 : Forward와 Right의 비율을 동일하게 맞춤
            cameraForward.Normalize();
            cameraRight.Normalize();   

            //이동 방향
            Vector3 moveDir = cameraForward * moveInput.y + cameraRight * moveInput.x; //WS 입력과 AD입력 방향 합치기
            moveDir.Normalize(); // 이동벡터의 속도를 1로 맞춤

            return moveDir;
        }

        //이동방향으로 캐릭터 회전
        private void Rotation(Vector3 moveDir)
        {
            if (moveDir.sqrMagnitude <= 0.001f) return; //이동입력이 거의 없으면 회전하지 않음

            Quaternion targetRotation = Quaternion.LookRotation(moveDir); //회전값

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        //중력 계산
        private void Gravity()
        {
            if (characterController.isGrounded && verticalVelocity < 0.0f)
            {
                verticalVelocity = -2.0f; //가만히 서있을땐 0.0f 이면 누르는 힘이 없어서 땅 판정이 불안정해서 isGrounded가 가끔 false남 . 그래서 -2 값으로 강제로 눌러줌
            }

            verticalVelocity += Physics.gravity.y * Time.deltaTime; //아래 방향 속도 증가
        }

        //점프
        private void Jump()
        {
            if (!playerInput.IsJump) return;
            if (!characterController.isGrounded) return; 

            verticalVelocity = jumpForce;
        }

        //회피
        public void Dodge(Vector3 dodgeDir)
        {
            Gravity();

            Vector3 velocity = dodgeDir * dodgeSpeed; //회피!
            velocity.y = verticalVelocity;

            characterController.Move(velocity * Time.deltaTime);
        }

        //질주

        public void Sprint()
        {
            MoveCharacter(sprintSpeed, true);
        }

        //피격
        private void DamageSlow()
        {
            //현재시간에 슬로우 지속시간을 더해서 = 슬로우가 끝나는 시간 저장
            damageSlowEndTime = Time.time + damageSlowDuration;

            Debug.Log($"슬로우시작 | 종료시간은 {damageSlowEndTime}");
        }
    }
}

