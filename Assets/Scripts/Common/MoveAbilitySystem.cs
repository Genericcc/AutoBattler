using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

namespace Common
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct MoveAbilitySystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (transform, abilityMoveSpeed) 
                     in SystemAPI.Query<RefRW<LocalTransform>, AbilityMoveSpeed>().WithAll<Simulate>())
            {
                transform.ValueRW.Position += transform.ValueRW.Forward() * abilityMoveSpeed.Value * deltaTime;
            }
        }
    }
}