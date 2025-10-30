using Imported.Samples.Character_Controller._1._3._12.Standard_Characters.ThirdPerson.Scripts.OrbitCamera;
using Unity.Burst;
using Unity.CharacterController;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Imported.Samples.Character_Controller._1._3._12.Standard_Characters.ThirdPerson.Scripts
{
    /// <summary>
    /// Apply inputs that need to be read at a variable rate
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct ThirdPersonPlayerVariableStepControlSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate(SystemAPI.QueryBuilder()
                .WithAll<ThirdPersonPlayer, ThirdPersonPlayerInputs>()
                .Build());
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (playerInputs, player) in SystemAPI
                         .Query<RefRO<ThirdPersonPlayerInputs>, RefRO<ThirdPersonPlayer>>()
                         .WithAll<Simulate>())
            {
                if (SystemAPI.HasComponent<OrbitCameraControl>(player.ValueRO.ControlledCamera))
                {
                    var cameraControl = SystemAPI.GetComponent<OrbitCameraControl>(player.ValueRO.ControlledCamera);

                    cameraControl.FollowedCharacterEntity = player.ValueRO.ControlledCharacter;
                    cameraControl.LookDegreesDelta = playerInputs.ValueRO.CameraLookInput;
                    cameraControl.ZoomDelta = playerInputs.ValueRO.CameraZoomInput;

                    SystemAPI.SetComponent(player.ValueRO.ControlledCamera, cameraControl);
                }
            }
        }
    }

    /// <summary>
    /// Apply inputs that need to be read at a fixed rate.
    /// It is necessary to handle this as part of the fixed step group, in case your framerate is lower than the fixed step rate.
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderFirst = true)]
    [BurstCompile]
    public partial struct ThirdPersonPlayerFixedStepControlSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Common.Scripts.FixedTickSystem.Singleton>();
            state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<ThirdPersonPlayer, ThirdPersonPlayerInputs>().Build());
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var tick = SystemAPI.GetSingleton<Common.Scripts.FixedTickSystem.Singleton>().Tick;

            foreach (var (playerInputs, player) in SystemAPI.Query<RefRO<ThirdPersonPlayerInputs>, RefRO<ThirdPersonPlayer>>().WithAll<Simulate>())
            {
                if (SystemAPI.HasComponent<ThirdPersonCharacterControl>(player.ValueRO.ControlledCharacter))
                {
                    var characterControl = SystemAPI.GetComponent<ThirdPersonCharacterControl>(player.ValueRO.ControlledCharacter);

                    var characterUp = MathUtilities.GetUpFromRotation(SystemAPI.GetComponent<LocalTransform>(player.ValueRO.ControlledCharacter).Rotation);

                    // Get camera rotation, since our movement is relative to it.
                    var cameraRotation = quaternion.identity;
                    if (SystemAPI.HasComponent<OrbitCamera.OrbitCamera>(player.ValueRO.ControlledCamera))
                    {
                        // Camera rotation is calculated rather than gotten from transform, because this allows us to
                        // reduce the size of the camera ghost state in a netcode prediction context.
                        // If not using netcode prediction, we could simply get rotation from transform here instead.
                        var orbitCamera = SystemAPI.GetComponent<OrbitCamera.OrbitCamera>(player.ValueRO.ControlledCamera);
                        cameraRotation = OrbitCameraUtilities.CalculateCameraRotation(characterUp, orbitCamera.PlanarForward, orbitCamera.PitchAngle);
                    }
                    var cameraForwardOnUpPlane = math.normalizesafe(MathUtilities.ProjectOnPlane(MathUtilities.GetForwardFromRotation(cameraRotation), characterUp));
                    var cameraRight = MathUtilities.GetRightFromRotation(cameraRotation);

                    // Move
                    characterControl.MoveVector = playerInputs.ValueRO.MoveInput.y * cameraForwardOnUpPlane + playerInputs.ValueRO.MoveInput.x * cameraRight;
                    characterControl.MoveVector = MathUtilities.ClampToMaxLength(characterControl.MoveVector, 1f);

                    // Jump
                    characterControl.Jump = playerInputs.ValueRO.JumpPressed.IsSet(tick);

                    SystemAPI.SetComponent(player.ValueRO.ControlledCharacter, characterControl);
                }
            }
        }
    }
}