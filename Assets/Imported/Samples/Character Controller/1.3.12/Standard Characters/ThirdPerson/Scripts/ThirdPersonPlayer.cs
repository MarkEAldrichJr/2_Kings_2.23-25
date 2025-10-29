using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Imported.Samples.Character_Controller._1._3._12.Standard_Characters.ThirdPerson.Scripts
{
    [Serializable]
    public struct ThirdPersonPlayer : IComponentData
    {
        public Entity ControlledCharacter;
        public Entity ControlledCamera;
    }

    [Serializable]
    public struct ThirdPersonPlayerInputs : IComponentData
    {
        public float2 MoveInput;
        public float2 CameraLookInput;
        public float CameraZoomInput;
        public FixedInputEvent JumpPressed;
    }
}