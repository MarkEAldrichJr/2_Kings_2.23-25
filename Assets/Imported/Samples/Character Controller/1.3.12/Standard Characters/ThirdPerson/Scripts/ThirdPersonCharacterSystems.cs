using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.CharacterController;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace Imported.Samples.Character_Controller._1._3._12.Standard_Characters.ThirdPerson.Scripts
{
    [UpdateInGroup(typeof(KinematicCharacterPhysicsUpdateGroup))]
    [BurstCompile]
    public partial struct ThirdPersonCharacterPhysicsUpdateSystem : ISystem
    {
        private EntityQuery _characterQuery;
        private ThirdPersonCharacterUpdateContext _context;
        private KinematicCharacterUpdateContext _baseContext;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _characterQuery = KinematicCharacterUtilities.GetBaseCharacterQueryBuilder()
                .WithAll<
                    ThirdPersonCharacterComponent,
                    ThirdPersonCharacterControl>()
                .Build(ref state);

            _context = new ThirdPersonCharacterUpdateContext();
            _context.OnSystemCreate();
            _baseContext = new KinematicCharacterUpdateContext();
            _baseContext.OnSystemCreate(ref state);

            state.RequireForUpdate(_characterQuery);
            state.RequireForUpdate<PhysicsWorldSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _context.OnSystemUpdate();
            _baseContext.OnSystemUpdate(ref state, SystemAPI.Time, SystemAPI.GetSingleton<PhysicsWorldSingleton>());

            var job = new ThirdPersonCharacterPhysicsUpdateJob
            {
                Context = _context,
                BaseContext = _baseContext
            };
            job.ScheduleParallel();
        }

        [BurstCompile]
        [WithAll(typeof(Simulate))]
        public partial struct ThirdPersonCharacterPhysicsUpdateJob : IJobEntity, IJobEntityChunkBeginEnd
        {
            public ThirdPersonCharacterUpdateContext Context;
            public KinematicCharacterUpdateContext BaseContext;

            private void Execute(ThirdPersonCharacterAspect characterAspect)
            {
                characterAspect.PhysicsUpdate(ref Context, ref BaseContext);
            }

            public bool OnChunkBegin(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                BaseContext.EnsureCreationOfTmpCollections();
                return true;
            }

            public void OnChunkEnd(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask, bool chunkWasExecuted)
            { }
        }
    }

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(ThirdPersonPlayerVariableStepControlSystem))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    [BurstCompile]
    public partial struct ThirdPersonCharacterVariableUpdateSystem : ISystem
    {
        private EntityQuery _characterQuery;
        private ThirdPersonCharacterUpdateContext _context;
        private KinematicCharacterUpdateContext _baseContext;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
            _characterQuery = KinematicCharacterUtilities.GetBaseCharacterQueryBuilder()
                .WithAll<
                    ThirdPersonCharacterComponent,
                    ThirdPersonCharacterControl>()
                .Build(ref state);

            _context = new ThirdPersonCharacterUpdateContext();
            _context.OnSystemCreate();
            _baseContext = new KinematicCharacterUpdateContext();
            _baseContext.OnSystemCreate(ref state);

            state.RequireForUpdate(_characterQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _context.OnSystemUpdate();
            _baseContext.OnSystemUpdate(ref state, SystemAPI.Time, SystemAPI.GetSingleton<PhysicsWorldSingleton>());

            var job = new ThirdPersonCharacterVariableUpdateJob
            {
                Context = _context,
                BaseContext = _baseContext
            };
            job.ScheduleParallel();
        }

        [BurstCompile]
        [WithAll(typeof(Simulate))]
        public partial struct ThirdPersonCharacterVariableUpdateJob : IJobEntity, IJobEntityChunkBeginEnd
        {
            public ThirdPersonCharacterUpdateContext Context;
            public KinematicCharacterUpdateContext BaseContext;

            private void Execute(ThirdPersonCharacterAspect characterAspect)
            {
                characterAspect.VariableUpdate(ref Context, ref BaseContext);
            }

            public bool OnChunkBegin(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                BaseContext.EnsureCreationOfTmpCollections();
                return true;
            }

            public void OnChunkEnd(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask, bool chunkWasExecuted)
            { }
        }
    }
}