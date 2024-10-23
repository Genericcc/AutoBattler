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

            foreach (var (localTransform, entity)
                     in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<DestroyEntityTag>().WithEntityAccess())
            {                
                //localTransform.ValueRW.Position = new float3(1000f, 1000f, 1000f);
                ecb.DestroyEntity(entity);
            }
        }
    }
}