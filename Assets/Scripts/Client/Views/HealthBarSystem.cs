using Common;

using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

using UnityEngine;
using UnityEngine.UIElements;

using Slider = UnityEngine.UI.Slider;

namespace Client.Views
{
    [UpdateAfter(typeof(TransformSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct HealthBarSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<UIPrefabs>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            //Spawn health bars for entities that require them
            foreach (var (transform, healthBarOffset, maxHitPoints, entity) 
                     in SystemAPI.Query<LocalTransform, HealthBarOffset, MaxHitPoints>()
                                 .WithNone<HealthBarUIReference>().WithEntityAccess())
            {
                var healthBarPrefab = SystemAPI.ManagedAPI.GetSingleton<UIPrefabs>().HealthBar;
                var spawnPosition = transform.Position + healthBarOffset.Value;
                var newHealthBar = Object.Instantiate(healthBarPrefab, spawnPosition, Quaternion.identity);

                SetHeathBar(newHealthBar, maxHitPoints.Value, maxHitPoints.Value);
                ecb.AddComponent(entity, new HealthBarUIReference { Value = newHealthBar });
            }

            //update pos and value of health bars
            foreach (var (transform, healthBarOffset, currentHitPoints, maxHitPoints, healthBarUIReference) 
                     in SystemAPI.Query<LocalTransform, HealthBarOffset, CurrentHitPoints, MaxHitPoints, HealthBarUIReference>())
            {
                var healthBarPos = transform.Position + healthBarOffset.Value;
                healthBarUIReference.Value.transform.position = healthBarPos;
                SetHeathBar(healthBarUIReference.Value, currentHitPoints.Value, maxHitPoints.Value);
            }
            
            //cleanup health bar once associated with entity that was destroyed (LocalTransform gets destroyed, but not our
            //HealthBarUIReference which is ICleanupComponentData)
            //Once all CleanupComponents are removed from the entity, it gets truly destroyed
            foreach (var (healthBarUIReference, entity) 
                     in SystemAPI.Query<HealthBarUIReference>().WithNone<LocalTransform>().WithEntityAccess())
            {
                Object.Destroy(healthBarUIReference.Value);
                ecb.RemoveComponent<HealthBarUIReference>(entity);
            }
        }

        private void SetHeathBar(GameObject healthBarCanvasObject, int currentHitPoints, int maxHitPoints)
        {
            var healthBarSlider = healthBarCanvasObject.GetComponentInChildren<Slider>();
            healthBarSlider.minValue = 0;
            healthBarSlider.maxValue = maxHitPoints;
            healthBarSlider.value = currentHitPoints;
        }
    }
    
}