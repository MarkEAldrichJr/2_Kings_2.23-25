using Unity.CharacterController;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Imported.Samples.Character_Controller._1._3._12.Standard_Characters.ThirdPerson.Scripts
{
    [DisallowMultipleComponent]
    public class ThirdPersonCharacterAuthoring : MonoBehaviour
    {
        public AuthoringKinematicCharacterProperties characterProperties = AuthoringKinematicCharacterProperties.GetDefault();

        public float rotationSharpness = 25f;
        public float groundMaxSpeed = 10f;
        public float groundedMovementSharpness = 15f;
        public float airAcceleration = 50f;
        public float airMaxSpeed = 10f;
        public float airDrag;
        public float jumpSpeed = 10f;
        public float3 gravity = math.up() * -30f;
        public bool preventAirAccelerationAgainstUngroundedHits = true;
        public BasicStepAndSlopeHandlingParameters stepAndSlopeHandling = BasicStepAndSlopeHandlingParameters.GetDefault();

        public class Baker : Baker<ThirdPersonCharacterAuthoring>
        {
            public override void Bake(ThirdPersonCharacterAuthoring authoring)
            {
                KinematicCharacterUtilities.BakeCharacter(this, authoring.gameObject, authoring.characterProperties);

                var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.WorldSpace);

                AddComponent(entity, new ThirdPersonCharacterComponent
                {
                    RotationSharpness = authoring.rotationSharpness,
                    GroundMaxSpeed = authoring.groundMaxSpeed,
                    GroundedMovementSharpness = authoring.groundedMovementSharpness,
                    AirAcceleration = authoring.airAcceleration,
                    AirMaxSpeed = authoring.airMaxSpeed,
                    AirDrag = authoring.airDrag,
                    JumpSpeed = authoring.jumpSpeed,
                    Gravity = authoring.gravity,
                    PreventAirAccelerationAgainstUngroundedHits = authoring.preventAirAccelerationAgainstUngroundedHits,
                    StepAndSlopeHandling = authoring.stepAndSlopeHandling
                });
                AddComponent(entity, new ThirdPersonCharacterControl());
            }
        }
    }
}