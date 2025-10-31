using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Mono
{
    public class MoveAvatarToCharacterController : MonoBehaviour
    {
        private PlayerReferenceStorage _playerReferenceStorage;
        private Transform _transAvatar;
        private EntityManager _entityManager;

        private void Awake()
        {
            _playerReferenceStorage = GetComponent<PlayerReferenceStorage>();
            _transAvatar = _playerReferenceStorage.GetPlayerAvatar.transform;
            
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        private void LateUpdate()
        {
            MoveToTransform();
        }

        private void MoveToTransform()
        {
            var transEntity =
                _entityManager.GetComponentData<LocalTransform>(_playerReferenceStorage.Character);

            _transAvatar.position = transEntity.Position;
            _transAvatar.rotation = transEntity.Rotation;
            _transAvatar.localScale = Vector3.one * transEntity.Scale;
        }
    }
}