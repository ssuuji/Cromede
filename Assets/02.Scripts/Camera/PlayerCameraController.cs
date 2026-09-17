using Cromede.Input;
using UnityEngine;

namespace Cromede.Camera
{
    public class PlayerCameraController : MonoBehaviour
    {
        [Header("카메라")]
        [SerializeField] private Transform player; //카메라 타겟
        [SerializeField] private float cameraRotation = 0.1f; //카메라 회전값
        [SerializeField] private float minRotation = -38.0f;  //위쪽 제한값
        [SerializeField] private float maxRotation = 50.0f;   //아래쪽 제한값

        private Vector3 offset; //플레이어와 카메라의 거리
        private PlayerInputSystem playerInput;
        private float verticalRotation; //위아래 회전 각도

        private void Awake()
        {
            offset = transform.position - player.position;
            playerInput = player.GetComponent<PlayerInputSystem>();
        }

        private void LateUpdate()
        {
            transform.position = player.position + offset;

            //좌우회전
            float mouseX = playerInput.LookAction.x; //좌우 이동값
            transform.RotateAround(player.position, Vector3.up, mouseX * cameraRotation); //Y축 기준 카메라 회전

            //상하회전
            float mouseY = playerInput.LookAction.y; //상하 이동값
            float rotationY = -mouseY * cameraRotation; //회전값
            float nextVerticalRotation = verticalRotation + rotationY;//회전각도+회전값

            nextVerticalRotation = Mathf.Clamp(nextVerticalRotation, minRotation, maxRotation); //범위 제한
            float amount = nextVerticalRotation - verticalRotation; //각도값

            transform.RotateAround(player.position, transform.right, amount); //회전
            
            verticalRotation = nextVerticalRotation; //회전 후 현재 각도 다시 저장
            offset = transform.position - player.position; //회전 후 위치 다시 저장
        }
    }
}

