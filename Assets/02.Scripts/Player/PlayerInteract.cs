using UnityEngine;

namespace Cromede.Player
{
    public class PlayerInteract : MonoBehaviour
    {
        [Header("상호작용")]
        [SerializeField] private Transform mainCamera;
        [SerializeField] private float interactRange = 3.0f;
        [SerializeField] private LayerMask interactLayer;

        //상호작용
        public bool Interact()
        {
            //카메라가 바라보는 방향 기준으로 상호작용 방향 설정
            Vector3 interactDir = mainCamera.forward;
            interactDir.y = 0.0f;
            interactDir.Normalize();

            //레이 시작 위치를 플레이어 위치보다 조금 위로 설정
            Vector3 ray = transform.position + Vector3.up;

            //상호작용 범위 안에 상호작용 가능한 오브젝트가 있는지 확인
            if (Physics.Raycast(ray, interactDir, out RaycastHit hit, interactRange, interactLayer))
            {
                Debug.Log($"{hit.collider.name}");
                return true;
            }

            return false;
        }
    }
}