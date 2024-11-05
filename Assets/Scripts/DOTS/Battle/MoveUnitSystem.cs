using DOTS.Battle.Curves;
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
            
            state.Dependency = new MoveToTargetJob
            {
                TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(),
                DeltaTime = deltaTime,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            }.ScheduleParallel(state.Dependency);
        }
    }

    [BurstCompile]
    public partial struct MoveToTargetJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;
        [ReadOnly] public float DeltaTime;

        public EntityCommandBuffer.ParallelWriter ECB;
        
        [BurstCompile]
        private void Execute(in TargetEntity target, ref MoveSpeed moveSpeed, ref CurveTimer curveTimer,
            in Entity targeter, [ChunkIndexInQuery] int sortKey)
        {
            if (!TransformLookup.HasComponent(target.Value))
            {
                moveSpeed.CurrentSpeedModifier = 0f;
                curveTimer.Reset();
                return;
            }

            var transform = TransformLookup[targeter];
            var currentTargetPosition = TransformLookup[target.Value].Position;
            
            if (math.distance(currentTargetPosition, transform.Position) <= 3f)
            {
                moveSpeed.CurrentSpeedModifier = 0f;
                curveTimer.Reset();
                return;
            }
            
            currentTargetPosition.y = transform.Position.y;
            var currentDir = math.normalizesafe(currentTargetPosition - transform.Position);
            transform.Position += currentDir * moveSpeed.CurrentSpeedModifier * moveSpeed.MaxSpeed * DeltaTime;
            transform.Rotation = quaternion.LookRotationSafe(currentDir, math.up());
            
            ECB.SetComponent(sortKey, targeter, transform);
        }
    }
}