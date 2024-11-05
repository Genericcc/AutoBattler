using Unity.Entities;
using Unity.Transforms;

namespace DOTS.Battle
{
    public partial struct DestroyEntitySystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (destroyEntityTag, entity)
                     in SystemAPI.Query<DestroyEntityTag>().WithEntityAccess())
            {                
                ecb.DestroyEntity(entity);
            }

            foreach (var (transform, entity)
                     in SystemAPI.Query<LocalTransform>().WithNone<DestroyEntityTag, CooldownToDestroy>().WithEntityAccess())
            {
                if (transform.Position.y < 0)
                {
                    ecb.AddComponent(entity, new CooldownToDestroy { Value = 1f });
                }
            }

            foreach (var (cooldownToDestroy, entity)
                     in SystemAPI.Query<CooldownToDestroy>().WithNone<DestroyEntityTag>().WithEntityAccess())
            {
                if (cooldownToDestroy.Value > 0)
                {
                    continue;
                }
                
                ecb.AddComponent<DestroyEntityTag>(entity);
            }
        }
    }
}