using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS.Battle
{
    public partial struct MoveUnitSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (transform, target, moveSpeed) 
                     in SystemAPI.Query<RefRW<LocalTransform>, TargetEntity, MoveSpeed>())
            {
                if (target.Target == Entity.Null)
                {
                    continue;
                }
                
                var currentTargetPosition = SystemAPI.GetComponent<LocalTransform>(target.Target).Position;
                if (math.distance(currentTargetPosition, transform.ValueRO.Position) <= 1.5f)
                {
                    continue;
                }

                currentTargetPosition.y = transform.ValueRO.Position.y;
                var currentDir = math.normalizesafe(currentTargetPosition - transform.ValueRO.Position);

                transform.ValueRW.Position += currentDir * moveSpeed.Value * deltaTime;
                transform.ValueRW.Rotation = quaternion.LookRotationSafe(currentDir, math.up());
            }
        }
    }
}