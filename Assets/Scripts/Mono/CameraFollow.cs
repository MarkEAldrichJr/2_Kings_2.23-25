using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Mono
{
    public class CameraFollow : MonoBehaviour
    {
        private PlayerReferenceStorage _playerRef;
        private EntityManager _entityMgr;
        
        private void Awake()
        {
            _playerRef = GetComponent<PlayerReferenceStorage>();
            _entityMgr = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        private void LateUpdate()
        {
            var targetTrans = _entityMgr.GetComponentData<LocalTransform>(_playerRef.OrbitCamera);

            _playerRef.GetPlayerCamera.transform.position = targetTrans.Position;
            _playerRef.GetPlayerCamera.transform.rotation = targetTrans.Rotation;
        }
    }
}