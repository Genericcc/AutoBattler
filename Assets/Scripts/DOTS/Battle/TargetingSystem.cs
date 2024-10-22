using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.Battle
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))]
    [UpdateBefore(typeof(ExportPhysicsWorld))]
    public partial struct TargetingSystem : ISystem
    {
        private CollisionFilter _npcAttackFilter;
        
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
            
            _npcAttackFilter = new CollisionFilter
            {
                BelongsTo = 1 << 6, //TargetCast
                CollidesWith = 1 << 1 //Units
            };
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new FindClosestTargetJob
            {
                CollisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld,
                CollisionFilter = _npcAttackFilter,
                TeamLookup = SystemAPI.GetComponentLookup<Team>(true),
            }.ScheduleParallel(state.Dependency);
        }
        
        [BurstCompile]
        public partial struct FindClosestTargetJob : IJobEntity
        {
            [ReadOnly] public CollisionWorld CollisionWorld;
            [ReadOnly] public CollisionFilter CollisionFilter;
            [ReadOnly] public ComponentLookup<Team> TeamLookup;

            [BurstCompile]
            private void Execute(Entity searchingEntity, ref TargetEntity targetEntity,
                in LocalTransform transform, in TargetingRadius searchRadius)
            {
                var hits = new NativeList<DistanceHit>(Allocator.TempJob);
                if (CollisionWorld.OverlapSphere(transform.Position, searchRadius.Value, ref hits, CollisionFilter))
                {
                    var closestDistance = float.MaxValue;
                    var closestEntity = Entity.Null;
                    
                    Debug.Log("Did overlap spehere");

                    foreach (var hit in hits)
                    {
                        if (!TeamLookup.TryGetComponent(hit.Entity, out var team)) { continue; }
                        if (TeamLookup[searchingEntity].Value == team.Value) { continue; }
                        
                        Debug.Log("Did find other team");

                        if (hit.Distance < closestDistance)
                        {
                            closestDistance = hit.Distance;
                            closestEntity = hit.Entity;
                        }
                    }

                    targetEntity.Target = closestEntity;
                }
                else
                {
                    Debug.Log("No overlap spehere, target null");
                    
                    targetEntity.Target = Entity.Null;
                }
                
                hits.Dispose();
            }
        }
        
    }
}