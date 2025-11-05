using Component;
using Imported.Samples.Character_Controller._1._3._12.Standard_Characters.ThirdPerson.Scripts;
using Imported.Samples.Character_Controller._1._3._12.Standard_Characters.ThirdPerson.Scripts.OrbitCamera;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Mono
{
    [RequireComponent(typeof(PlayerReferenceStorage))]
    public class PlayerCharacterControllerSpawn : MonoBehaviour
    {
        [SerializeField] private float idleWalkThreshold = 0.5f;
        [SerializeField] private float walkRunThreshold = 1.5f;
        
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
                SpawnCharacterControllerEntities(prefabs);
                MatchCharacterToAnimatorSpawn();
            }
        }

        private void SpawnCharacterControllerEntities(EntityPrefabComponent prefabs)
        {
            _refStorage.Character = _entityManager.Instantiate(prefabs.ThirdPersonCharacter);
            _refStorage.OrbitCamera = _entityManager.Instantiate(prefabs.OrbitCamera);
            _refStorage.Player = _entityManager.Instantiate(prefabs.ThirdPersonPlayer);
                
            _entityManager.SetComponentData(_refStorage.Player, new ThirdPersonPlayer
            {
                ControlledCharacter = _refStorage.Character,
                ControlledCamera = _refStorage.OrbitCamera
            });

            //Adding animState to Character over Player.  Character has direct reference to Ground state
            _entityManager.AddComponentData(_refStorage.Character, new AnimationStateComp{
                IdleWalkThreshold = idleWalkThreshold,
                WalkRunThreshold = walkRunThreshold
            });
            
            if (!_refStorage.GetPlayerAnimator)
            {
                _entityManager.AddComponentObject(_refStorage.Character,
                    _refStorage.GetPlayerAnimator);
            }

            _refStorage.CameraTarget = 
                _entityManager.GetComponentData<CameraTarget>(_refStorage.Character).TargetEntity;
        }

        private void MatchCharacterToAnimatorSpawn()
        {
            var spawnEntities = GetPlayerSpawnEntities(_entityManager);
            var spawnEntity = spawnEntities[Random.Range(0, spawnEntities.Length)];
            var spawnTransform = _entityManager.GetComponentData<LocalToWorld>(spawnEntity);
            
            _entityManager.SetComponentData(_refStorage.Character, new LocalTransform
            {
                Scale = 1f,
                Position = spawnTransform.Position,
                Rotation = spawnTransform.Rotation
            });
        }
        
        private static NativeArray<Entity> GetPlayerSpawnEntities(EntityManager entityManager)
        {
            var query = entityManager.CreateEntityQuery(new EntityQueryBuilder(Allocator.Temp)
                            .WithAll<PlayerSpawnTag>());
            
            return query.ToEntityArray(Allocator.Temp);
        }
    }
}