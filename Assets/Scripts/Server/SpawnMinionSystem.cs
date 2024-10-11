using Common;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Server
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct SpawnMinionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GamePlayingTag>();
            state.RequireForUpdate<MinionPathContainers>();
            state.RequireForUpdate<MobaPrefabs>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var minionSpawnAspect in SystemAPI.Query<MinionSpawnAspect>())
            {
                minionSpawnAspect.DecrementTimers(deltaTime);
                if (minionSpawnAspect.ShouldSpawn)
                {
                    SpawnOnEachLane(ref state);
                    minionSpawnAspect.CountSpawnedInWave++;

                    if (minionSpawnAspect.IsWaveSpawned)
                    {
                        minionSpawnAspect.ResetMinionTimer();
                        minionSpawnAspect.ResetWaveTimer();
                        minionSpawnAspect.ResetSpawnCounter();
                    }
                    else
                    {
                        minionSpawnAspect.ResetMinionTimer();
                    }
                }
            }
        }

        private void SpawnOnEachLane(ref SystemState state)
        {
            //This line won't work without ref SystemState (that's what Turbo has said)
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var minionPrefab = SystemAPI.GetSingleton<MobaPrefabs>().Minion;
            var pathContainers = SystemAPI.GetSingleton<MinionPathContainers>();

            var topLane = SystemAPI.GetBuffer<MinionPathPosition>(pathContainers.TopLane);
            SpawnMinionOnLane(ecb, minionPrefab, topLane);

            var midLane = SystemAPI.GetBuffer<MinionPathPosition>(pathContainers.MidLane);
            SpawnMinionOnLane(ecb, minionPrefab, midLane);

            var botLane = SystemAPI.GetBuffer<MinionPathPosition>(pathContainers.BotLane);
            SpawnMinionOnLane(ecb, minionPrefab, botLane);
        }

        private void SpawnMinionOnLane(EntityCommandBuffer ecb, Entity minionPrefab, DynamicBuffer<MinionPathPosition> lane)
        {
            var newBlueMinion = ecb.Instantiate(minionPrefab);
            for (var i = 0; i < lane.Length; i++)
            {
                ecb.AppendToBuffer(newBlueMinion, lane[i]);
            }

            var blueSpawnTransform = LocalTransform.FromPosition(lane[0].Value);
            ecb.SetComponent(newBlueMinion, blueSpawnTransform);
            ecb.SetComponent(newBlueMinion, new MobaTeam { Value = TeamType.Blue });
            
            var newRedMinion = ecb.Instantiate(minionPrefab);
            for (var i = lane.Length - 1; i >= 0; i--)
            {
                ecb.AppendToBuffer(newRedMinion, lane[i]);
            }

            var redSpawnTransform = LocalTransform.FromPosition(lane[^1].Value);
            ecb.SetComponent(newRedMinion, redSpawnTransform);
            ecb.SetComponent(newRedMinion, new MobaTeam { Value = TeamType.Red });
        }
    }
}