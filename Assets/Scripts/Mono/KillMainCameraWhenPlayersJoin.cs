using UnityEngine;
using UnityEngine.InputSystem;

namespace Mono
{
    public class KillMainCameraWhenPlayersJoin : MonoBehaviour
    {
        [SerializeField] private GameObject cam;
        public uint NumPlayers { private set; get; }
        
        private void OnPlayerJoined(PlayerInput input)
        {
            NumPlayers++;
            ActivateCamera();
        }

        private void OnPlayerLeft(PlayerInput input)
        {
            NumPlayers--;
            ActivateCamera();
        }

        private void ActivateCamera()
        {
            cam?.SetActive(NumPlayers == 0);
        }
    }
}
