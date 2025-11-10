using System;
using Unity.CharacterController;
using Unity.Entities;
using Unity.Mathematics;

namespace Imported.Samples.Character_Controller._1._3._12.Standard_Characters.ThirdPerson.Scripts
{
    [Serializable]
    public struct ThirdPersonCharacterComponent : IComponentData
    {
        public float RotationSharpness;
        public float GroundMaxSpeed;
        public float GroundedMovementSharpness;
        public float AirAcceleration;
        public float AirMaxSpeed;
        public float AirDrag;
        public float JumpSpeed;
        public float3 Gravity;
        public bool PreventAirAccelerationAgainstUngroundedHits;
        public BasicStepAndSlopeHandlingParameters StepAndSlopeHandling;
    }

    [Serializable]
    public struct ThirdPersonCharacterControl : IComponentData
    {
        public float3 MoveVector;
        public bool Jump;
        public bool Attack;
        public bool Sleep;
    }
}