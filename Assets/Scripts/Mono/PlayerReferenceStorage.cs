using Unity.Entities;
using UnityEngine;

namespace Mono
{
    public class PlayerReferenceStorage : MonoBehaviour
    {
        [Header("Game Objects")]
        [SerializeField] private GameObject playerCamera;
        [SerializeField] private GameObject playerAvatar;
        [SerializeField] private Animator playerAnimator;
        [SerializeField] private EntityTransformFollower entityTransformFollower;
        
        public GameObject GetPlayerCamera => playerCamera;
        public GameObject GetPlayerAvatar => playerAvatar;
        public Animator GetPlayerAnimator => playerAnimator;
        public EntityTransformFollower GetEntityTransformFollower => entityTransformFollower;

        public Entity CameraTarget;
        public Entity OrbitCamera;
        public Entity Character;
        public Entity Player;
    }
}
