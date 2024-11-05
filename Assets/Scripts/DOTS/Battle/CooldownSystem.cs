using Unity.Burst;
using Unity.Entities;

namespace DOTS.Battle
{
    public partial struct CooldownSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            
            foreach (var currentCooldown in SystemAPI.Query<RefRW<CurrentCooldown>>())
            {
                if (currentCooldown.ValueRO.Value > 0)
                {
                    currentCooldown.ValueRW.Value -= deltaTime;
                }
            }
            
            foreach (var cooldownToDestroy in SystemAPI.Query<RefRW<CooldownToDestroy>>())
            {
                if (cooldownToDestroy.ValueRO.Value > 0)
                {
                    cooldownToDestroy.ValueRW.Value -= deltaTime;
                }
            }
        }
    }
}