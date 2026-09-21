using Cromede.Input;
using UnityEngine;

namespace Cromede.Camera
{
    public class PlayerCameraController : MonoBehaviour
    {
        [Header("카메라")]
        [SerializeField] private Transform player;            
        [SerializeField] private float cameraRotation = 0.1f; //카메라 회전값
        [SerializeField] private float minRotation = -38.0f;  //위쪽 제한값
        [SerializeField] private float maxRotation = 50.0f;   //아래쪽 제한값

        [Header("줌")]
        [SerializeField] private Transform cameraTarget;      //플레이어 머리
        [SerializeField] private float minZoomDistance = 2.0f;
        [SerializeField] private float maxZoomDistance = 10.0f;
        [SerializeField] private float zoomStep = 1.0f;
        [SerializeField] private float zoomSmoothSpeed = 8.0f;
        private float targetZoomDistance;


        private Vector3 offset; //플레이어와 카메라의 거리
        private PlayerInputSystem playerInput;
        private float verticalRotation; //위아래 회전 각도

        private void Awake()
        {
            Vector3 playerDir = cameraTarget.position - transform.position;
            playerDir.y = 0.0f;
            float cameraY = Quaternion.LookRotation(playerDir).eulerAngles.y;
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, cameraY, 0.0f);

            playerInput = player.GetComponent<PlayerInputSystem>();

            offset = transform.position - cameraTarget.position;
            targetZoomDistance = offset.magnitude; //플레이어와 카메라 사이 거리

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void LateUpdate()
        {
            transform.position = cameraTarget.position + offset;

            //Alt 마우스 보이기
            if (playerInput.IsCursorMode)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                return;
            }
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            //좌우회전
            float mouseX = playerInput.LookAction.x; //좌우 이동값
            transform.RotateAround(cameraTarget.position, Vector3.up, mouseX * cameraRotation); //Y축 기준 카메라 회전

            //상하회전
            float mouseY = playerInput.LookAction.y; //상하 이동값
            float rotationY = -mouseY * cameraRotation; //회전값
            float nextVerticalRotation = verticalRotation + rotationY;//회전각도+회전값

            nextVerticalRotation = Mathf.Clamp(nextVerticalRotation, minRotation, maxRotation); //범위 제한
            float amount = nextVerticalRotation - verticalRotation; //각도값

            transform.RotateAround(cameraTarget.position, transform.right, amount); //회전
            
            verticalRotation = nextVerticalRotation; //회전 후 현재 각도 다시 저장
            offset = transform.position - cameraTarget.position; //회전 후 위치 다시 저장

            //줌
            float zoomInput = playerInput.ZoomAction;
            if (zoomInput != 0.0f)
            {
                targetZoomDistance -= zoomInput * zoomStep; //-= : 밑으로 스크롤(-1)하면 거리가 증가해서 화면이 멀어짐
                targetZoomDistance = Mathf.Clamp(targetZoomDistance, minZoomDistance, maxZoomDistance);
            }
            float currentZoomDistance = offset.magnitude;
            float zoomDistacne = Mathf.Lerp(currentZoomDistance, targetZoomDistance, zoomSmoothSpeed * Time.deltaTime);
            offset = offset.normalized * zoomDistacne;
            transform.position = cameraTarget.position + offset;
        }
    }
}

