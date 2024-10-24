using DOTS.Rounds;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS.Battle
{
    public partial struct AttackSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var roundState in SystemAPI.Query<RoundState>())
            {
                if (roundState.RoundStateType != RoundStateType.Playing)
                {
                    return;
                }
            }
            
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            
            state.Dependency = new AttackJob
            {
                TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(),
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            }.ScheduleParallel(state.Dependency);
        }
    }

    [BurstCompile]
    [WithAll(typeof(Simulate))]
    public partial struct AttackJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;

        public EntityCommandBuffer.ParallelWriter ECB;

        [BurstCompile]
        private void Execute(ref CurrentCooldown attackCooldown, in AttackProperties attackProperties, 
            in TargetEntity targetEntity, Entity npcEntity, Team team, [ChunkIndexInQuery] int sortKey)
        {
            if (targetEntity.Value == Entity.Null || !TransformLookup.HasComponent(targetEntity.Value))
            {
                return;
            }

            if (attackCooldown.Value > 0)
            {
                return;
            }

            var spawnPosition = TransformLookup[npcEntity].Position + attackProperties.FirePointOffset;
            var targetPosition = TransformLookup[targetEntity.Value].Position;

            var newAttack = ECB.Instantiate(sortKey, attackProperties.AttackPrefab);
            var newAttackTransform = LocalTransform.FromPositionRotation(spawnPosition,
                quaternion.LookRotationSafe(targetPosition - spawnPosition, math.back()));
            
            var newCooldown = new CurrentCooldown { Value = attackProperties.Cooldown }; 
            
            ECB.SetComponent(sortKey, newAttack, newAttackTransform);
            ECB.SetComponent(sortKey, npcEntity, newCooldown);
            ECB.SetComponent(sortKey, newAttack, team);
        }
    }
}