using Imported.Samples.Character_Controller._1._3._12.Standard_Characters.ThirdPerson.Scripts;
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

        private void Awake()
        {
            _playerRef = GetComponent<PlayerReferenceStorage>();
        }

        private void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        public void OnDeviceLost()
        {
            DebugLog("Device Lost");
        }

        public void OnDeviceRegained()
        {
            DebugLog("Device Regained");
        }

        public void OnControlsChanged()
        {
            DebugLog("Controls Changed");
        }
    
        public void OnMove(InputValue value)
        {
            var moveValue = value.Get<Vector2>();
            DebugLog($"Move Stick Value: {moveValue.ToString()}");
            
            var inputs = _entityManager.GetComponentData<ThirdPersonPlayerInputs>(_playerRef.Player);
            inputs.MoveInput = moveValue;
            _entityManager.SetComponentData(_playerRef.Player, inputs);

            //var magnitude = math.distance(moveValue, float2.zero);
            
            //get animator
            //if magnitude is > x run, if <= x walk
        }
        
        public void OnLook(InputValue value)
        {
            var lookValue = value.Get<Vector2>();
            DebugLog($"Look Stick Value: {lookValue.ToString()}");
            
            var inputs = _entityManager.GetComponentData<ThirdPersonPlayerInputs>(_playerRef.Player);
            inputs.CameraLookInput = lookValue;
            _entityManager.SetComponentData(_playerRef.Player, inputs);
        }
        
        public void OnAttack(InputValue value)
        {
        
        }
        
        public void OnJump(InputValue value)
        {
        
        }

        private void DebugLog(string message)
        {
            if (debug)
            {
                Debug.Log(message);
            }
        }
    }
}
