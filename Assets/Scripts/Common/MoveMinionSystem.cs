using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace Common
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct MoveMinionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GamePlayingTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (transform, pathIndex, pathPositions, moveSpeed) 
                     in SystemAPI.Query<RefRW<LocalTransform>, RefRW<MinionPathIndex>, 
                             DynamicBuffer<MinionPathPosition>, CharacterMoveSpeed>().WithAll<Simulate>())

            {
                var currentTargetPosition = pathPositions[pathIndex.ValueRO.Value].Value;
                if (math.distance(currentTargetPosition, transform.ValueRO.Position) <= 1.5f)
                {
                    if (pathIndex.ValueRO.Value >= pathPositions.Length - 1) continue;
                    pathIndex.ValueRW.Value++;
                    currentTargetPosition = pathPositions[pathIndex.ValueRO.Value].Value;
                }

                currentTargetPosition.y = transform.ValueRO.Position.y;
                var currentDir = math.normalizesafe(currentTargetPosition - transform.ValueRO.Position);

                transform.ValueRW.Position += currentDir * moveSpeed.Value * deltaTime;
                transform.ValueRW.Rotation = quaternion.LookRotationSafe(currentDir, math.up());
            }
        }
    }
}