using DOTS.Rounds;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS.Battle
{
    public partial struct MoveUnitSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var roundState in SystemAPI.Query<RoundState>())
            {
                if (roundState.RoundStateType != RoundStateType.Playing)
                {
                    return;
                }
            }
            
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var deltaTime = SystemAPI.Time.DeltaTime;
            
            state.Dependency = new UpdateTargetJob
            {
                DeltaTime = deltaTime,
                TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(),
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            }.ScheduleParallel(state.Dependency);
        }
    }
    
    [BurstCompile]
    public partial struct UpdateTargetJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;
        [ReadOnly] public float DeltaTime;

        public EntityCommandBuffer.ParallelWriter ECB;
        
        [BurstCompile]
        public void Execute(in TargetEntity target, in MoveSpeed moveSpeed, in Entity targeter, [ChunkIndexInQuery] int sortKey)
        {
            if (!TransformLookup.HasComponent(target.Value))
            {
                return;
            }

            var transform = TransformLookup[targeter];
            
            var currentTargetPosition = TransformLookup[target.Value].Position;
            if (math.distance(currentTargetPosition, transform.Position) <= 3f)
            {
                return;
            }
            
            currentTargetPosition.y = transform.Position.y;
            var currentDir = math.normalizesafe(currentTargetPosition - transform.Position);
            transform.Position += currentDir * moveSpeed.Value * DeltaTime;
            transform.Rotation = quaternion.LookRotationSafe(currentDir, math.up());
            
            ECB.SetComponent(sortKey, targeter, transform);
        }
    }
}