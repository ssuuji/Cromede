using Cromede.Player.State;
using UnityEngine;
using static Cromede.Player.State.PlayerStateMachine;

namespace Cromede.Player
{
    public class PlayerInteract : MonoBehaviour
    {
        [Header("상호작용")]
        [SerializeField] private Transform mainCamera;
        [SerializeField] private float interactRange = 3.0f;
        [SerializeField] private LayerMask interactLayer;

        private PlayerStateMachine stateMachine;

        private void Awake()
        {
            stateMachine = GetComponent<PlayerStateMachine>();
        }

        //상호작용
        public void Interact()
        {
            Vector3 interactDir = mainCamera.forward;
            interactDir.y = 0.0f;
            interactDir.Normalize();

            Vector3 ray = transform.position + Vector3.up;
            if (Physics.Raycast(ray, interactDir, out RaycastHit hit, interactRange, interactLayer))
            {
                Debug.Log($"{hit.collider.name}");
                stateMachine.ChangeState(PlayerStateType.Interact);
            }
        }
    }
}

