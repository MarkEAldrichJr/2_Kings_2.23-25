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
            //destroy the prefab, its control and transform entities
        }

        public void OnDeviceRegained()
        {
            //spawn prefab anew
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

            var magnitude = math.distance(moveValue, float2.zero);
            
            //get animator
            //if magnitude is > x run, if <= x walk
        }

        public void OnLook(InputValue value)
        {
            //Rotate camera
        }

        public void OnAttack(InputValue value)
        {
        
        }

        public void OnInteract(InputValue value)
        {
        
        }

        public void OnCrouch(InputValue value)
        {
        
        }

        public void OnJump(InputValue value)
        {
        
        }

        public void OnSprint(InputValue value)
        {
        
        }
    
    }
}
