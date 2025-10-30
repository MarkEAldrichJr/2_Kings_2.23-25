using Component;
using Imported.Samples.Character_Controller._1._3._12.Standard_Characters.ThirdPerson.Scripts;
using Imported.Samples.Character_Controller._1._3._12.Standard_Characters.ThirdPerson.Scripts.OrbitCamera;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Mono
{
    [RequireComponent(typeof(PlayerReferenceStorage))]
    public class PlayerCharacterControllerSpawn : MonoBehaviour
    {
        private PlayerReferenceStorage _refStorage;
        private EntityManager _entityManager;

        private void Awake() => _refStorage = GetComponent<PlayerReferenceStorage>();

        /// <summary>
        /// Spawn in the three major Character Controller entities, and
        /// populate the relevant references.
        /// </summary>
        private void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entityQuery = _entityManager.CreateEntityQuery(
                new EntityQueryBuilder(Allocator.Temp)
                .WithAll<EntityPrefabComponent>());
            
            if (entityQuery.TryGetSingleton<EntityPrefabComponent>(out var prefabs))
            {
                _refStorage.Character = _entityManager.Instantiate(prefabs.ThirdPersonCharacter);
                _refStorage.OrbitCamera = _entityManager.Instantiate(prefabs.OrbitCamera);
                _refStorage.Player = _entityManager.Instantiate(prefabs.ThirdPersonPlayer);
                
                _entityManager.SetComponentData(_refStorage.Player, new ThirdPersonPlayer
                {
                    ControlledCharacter = _refStorage.Character,
                    ControlledCamera = _refStorage.OrbitCamera
                });

                _refStorage.CameraTarget = 
                    _entityManager.GetComponentData<CameraTarget>(_refStorage.Character).TargetEntity;
            }
        }
    }
}