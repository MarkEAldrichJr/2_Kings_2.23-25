using UnityEngine;
using UnityEngine.InputSystem;

namespace Mono
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputController : MonoBehaviour
    {
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
            //get our entity
            //Set MoveDirection values to match this
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
