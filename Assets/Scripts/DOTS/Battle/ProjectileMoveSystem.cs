using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace DOTS.Battle
{
    public partial struct ProjectileMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (transform, projectileSpeed) 
                     in SystemAPI.Query<RefRW<LocalTransform>, ProjectileSpeed>().WithAll<Simulate>())
            {
                transform.ValueRW.Position += transform.ValueRW.Forward() * projectileSpeed.Value * deltaTime;
            }
        } 
    }
}