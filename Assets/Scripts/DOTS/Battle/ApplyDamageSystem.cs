using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace DOTS.Battle
{
    public partial struct ApplyDamageSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp); 
            
            foreach (var (currentHealth, damageBuffer, entity) 
                     in SystemAPI.Query<RefRW<CurrentHealth>, DynamicBuffer<DamageBufferElement>>().WithEntityAccess())
            {
                if (damageBuffer.Length <= 0)
                {
                    continue;
                }

                var damageThisFrame = 0;

                foreach (var damageBufferElement in damageBuffer)
                {
                    damageThisFrame += damageBufferElement.Value;
                }

                currentHealth.ValueRW.Value -= damageThisFrame;

                if (currentHealth.ValueRW.Value <= 0)
                {
                    ecb.AddComponent<DestroyEntityTag>(entity);
                }
                
                damageBuffer.Clear();
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}