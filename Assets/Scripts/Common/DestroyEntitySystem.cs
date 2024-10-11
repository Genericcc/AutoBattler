using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace Common
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
    public partial struct DestroyEntitySystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MobaPrefabs>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<NetworkTime>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();

            if (!networkTime.IsFirstTimeFullyPredictingTick)
            {
                return;
            }

            var currentTick = networkTime.ServerTick;

            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (localTransform, entity) 
                     in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<DestroyEntityTag, Simulate>().WithEntityAccess())
            {
                if (state.World.IsServer())
                {
                    if (SystemAPI.HasComponent<GameOverOnDestroyTag>(entity))
                    {
                        var gameOverPrefab = SystemAPI.GetSingleton<MobaPrefabs>().GameOverEntity;
                        var gameOverEntity = ecb.Instantiate(gameOverPrefab);

                        var losing = SystemAPI.GetComponent<MobaTeam>(entity).Value;
                        var winningTeam = losing == TeamType.Blue ? TeamType.Red : TeamType.Blue;
                        
                        Debug.Log($"{winningTeam} Team won!");
                        
                        ecb.SetName(gameOverEntity, "GameOverEntity");
                        ecb.SetComponent(gameOverEntity, new WinningTeam { Value = winningTeam });
                    }
                    
                    ecb.DestroyEntity(entity);
                }
                else
                {
                    localTransform.ValueRW.Position = new float3(1000f, 1000f, 1000f);
                }
            }  
        }
    }
}