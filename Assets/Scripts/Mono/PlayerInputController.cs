using Imported.Samples.Character_Controller._1._3._12.Standard_Characters.Common.Scripts;
using Imported.Samples.Character_Controller._1._3._12.Standard_Characters.ThirdPerson.Scripts;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mono
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputController : MonoBehaviour
    {
        [SerializeField] private bool debug;
        private PlayerReferenceStorage _playerRef;
        private EntityManager _entityManager;
        private EntityQuery _fixedTickQuery;

        private void Awake()
        {
            _playerRef = GetComponent<PlayerReferenceStorage>();
        }

        private void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _fixedTickQuery = _entityManager.CreateEntityQuery(
                            new EntityQueryBuilder(Allocator.Temp)
                                .WithAll<FixedTickSystem.FixedTickSingleton>());
        }

        public void OnDeviceLost()
        {
            DebugLog(debug, "Device Lost");
        }

        public void OnDeviceRegained()
        {
            DebugLog(debug, "Device Regained");
        }

        public void OnControlsChanged()
        {
            DebugLog(debug, "Controls Changed");
        }
    
        public void OnMove(InputValue value)
        {
            var moveValue = value.Get<Vector2>();
            
            var inputs = _entityManager.GetComponentData<ThirdPersonPlayerInputs>(_playerRef.Player);
            inputs.MoveInput = moveValue;
            _entityManager.SetComponentData(_playerRef.Player, inputs);
        }
        
        public void OnLook(InputValue value)
        {
            var lookValue = value.Get<Vector2>();
            
            var inputs = _entityManager.GetComponentData<ThirdPersonPlayerInputs>(_playerRef.Player);
            inputs.CameraLookInput = lookValue;
            _entityManager.SetComponentData(_playerRef.Player, inputs);
        }
        
        public void OnAttack(InputValue value)
        {
            if (!_entityManager.Exists(_playerRef.Player)) return;
            var inputs = _entityManager.GetComponentData<ThirdPersonPlayerInputs>(_playerRef.Player);
            var tick = _fixedTickQuery.GetSingleton<FixedTickSystem.FixedTickSingleton>();
            inputs.AttackPressed.Set(tick.Tick);
            
            _entityManager.SetComponentData(_playerRef.Player, inputs);
        }
        
        public void OnJump(InputValue value)
        {
            var inputs = _entityManager.GetComponentData<ThirdPersonPlayerInputs>(_playerRef.Player);
            var tick = _fixedTickQuery.GetSingleton<FixedTickSystem.FixedTickSingleton>();
            inputs.JumpPressed.Set(tick.Tick);
            
            _entityManager.SetComponentData(_playerRef.Player, inputs);
        }

        private static void DebugLog(bool shouldDebug, string message)
        {
            if (shouldDebug)
            {
                Debug.Log(message);
            }
        }
    }
}
