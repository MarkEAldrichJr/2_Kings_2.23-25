using Imported.Samples.Character_Controller._1._3._12.Standard_Characters.ThirdPerson.Scripts;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mono
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputController : MonoBehaviour
    {
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
            Debug.Log("Device Lost");
        }

        public void OnDeviceRegained()
        {
            Debug.Log("Device Regained");
        }

        public void OnControlsChanged()
        {
            Debug.Log("Controls Changed");
        }
    
        public void OnMove(InputValue value)
        {
            var moveValue = value.Get<Vector2>();
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
    }
}
