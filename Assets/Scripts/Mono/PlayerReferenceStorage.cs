using Unity.Entities;
using UnityEngine;

namespace Mono
{
    public class PlayerReferenceStorage : MonoBehaviour
    {
        [Header("Game Objects")]
        [SerializeField] private GameObject playerCamera;
        [SerializeField] private GameObject playerAvatar;
        [SerializeField] private AnimatorController playerAnimator;
        
        public GameObject GetPlayerCamera => playerCamera;
        public GameObject GetPlayerAvatar => playerAvatar;
        public AnimatorController GetPlayerAnimator => playerAnimator;

        public Entity CameraTarget;
        public Entity OrbitCamera;
        public Entity Character;
        public Entity Player;
    }
}
