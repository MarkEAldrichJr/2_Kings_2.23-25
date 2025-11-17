using Component;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class PrefabAuthoring : MonoBehaviour
    {
        [Header("Player")]
        public GameObject characterPrefab;
        public GameObject orbitCameraPrefab;
        public GameObject playerPrefab;
        
        [Header("NPCs")]
        public GameObject baseChildPrefab;
        
        public class PrefabAuthoringBaker : Baker<PrefabAuthoring>
        {
            public override void Bake(PrefabAuthoring authoring)
            {
                var characterEntity = GetEntity(authoring.characterPrefab, TransformUsageFlags.Dynamic);
                var orbitCameraEntity = GetEntity(authoring.orbitCameraPrefab, TransformUsageFlags.Dynamic);
                var playerEntity = GetEntity(authoring.playerPrefab, TransformUsageFlags.None);
                
                var baseChildEntity = GetEntity(authoring.baseChildPrefab, TransformUsageFlags.Dynamic);

                var prefabStorageEntity = GetEntity(TransformUsageFlags.None);
                
                AddComponent(prefabStorageEntity, new EntityPrefabComponent
                {
                    OrbitCamera = orbitCameraEntity,
                    ThirdPersonCharacter = characterEntity,
                    ThirdPersonPlayer = playerEntity,
                    
                    BaseChild = baseChildEntity
                });
            }
        }
    }
}