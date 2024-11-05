using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace DOTS.Battle.Curves
{
    public partial struct InitializeAccelerationCurveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BlobAssetContainerTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var gameControllerEntity = SystemAPI.GetSingletonEntity<BlobAssetContainerTag>();
            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            foreach (var (accelerationTimer, entity) in SystemAPI.Query<CurveTimer>()
                         .WithNone<AccelerationCurveReference>()
                         .WithEntityAccess())
            {
                var accelerationCurveReference = SystemAPI.GetComponent<AccelerationCurveReference>(gameControllerEntity);
                ecb.AddComponent<AccelerationCurveReference>(entity);
                ecb.SetComponent(entity, accelerationCurveReference);
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}