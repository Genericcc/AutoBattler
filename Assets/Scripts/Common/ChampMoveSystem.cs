using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace Common
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct ChampMoveSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GamePlayingTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (entityTransform, movePosition, moveSpeed) 
                     in SystemAPI.Query<RefRW<LocalTransform>, ChampMoveTargetPosition, CharacterMoveSpeed>().WithAll<Simulate>())
            {
                var moveTarget = movePosition.Value;
                moveTarget.y = entityTransform.ValueRO.Position.y;

                if (math.distancesq(entityTransform.ValueRO.Position, moveTarget) < 0.001f)
                {
                    continue;
                }
                
                var moveDirection = math.normalizesafe(moveTarget - entityTransform.ValueRO.Position);
                var moveVector = moveDirection * moveSpeed.Value * deltaTime;
                
                entityTransform.ValueRW.Position += moveVector;
                entityTransform.ValueRW.Rotation = quaternion.LookRotationSafe(moveDirection, math.up());
            }
        }
    }
}