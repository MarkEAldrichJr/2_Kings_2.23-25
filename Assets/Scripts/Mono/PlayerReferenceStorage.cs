using Unity.Entities;
using UnityEngine;

namespace Mono
{
    public class PlayerReferenceStorage : MonoBehaviour
    {
        [Header("Game Objects")]
        [SerializeField] private GameObject playerCamera;
        
        public GameObject GetPlayerCamera => playerCamera;

        public Entity OrbitCamera;
        public Entity Character;
        public Entity Player;
    }
}
