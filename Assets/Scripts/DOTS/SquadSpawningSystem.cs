using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTS
{
    public partial struct SquadSpawningSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SquadElement>();
            state.RequireForUpdate<BattleGridTag>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            
            var spawnOrders = SystemAPI.GetSingletonBuffer<SquadSpawnOrder>();
            if (spawnOrders.Length <= 0)
            {
                return;
            }
            
            var availableSquads = SystemAPI.GetSingletonBuffer<SquadElement>();

            for (var i = 0; i < spawnOrders.Length; i++)
            {
                if (TryGetSquad(availableSquads, spawnOrders[i].SquadID, out var squadElement))
                {
                    var e = ecb.Instantiate(squadElement.Prefab);
                    SpawnSquad(ref state, squadElement.Size, out var spawnPosition);
                    ecb.SetComponent(e, spawnPosition);
                    
                    Debug.Log($"Spawned!");
                }
            }
            
            spawnOrders.Clear();
        }

        private void SpawnSquad(ref SystemState state, int2 squadSize, out LocalTransform spawnPosition)
        {
            var battleGridEntity = SystemAPI.GetSingletonEntity<BattleGridTag>();
            var gridNodes = SystemAPI.GetBuffer<GridNode>(battleGridEntity);
            var gridProperties = SystemAPI.GetComponent<BattleGridProperties>(battleGridEntity);

            var squadNodes = new NativeList<GridNode>(Allocator.Temp);

            var gridSize = new int2(gridProperties.Width, gridProperties.Height);
            int x = 0, y = 0;
            int dx = 0, dy = -1;
            var maxVal = math.max(gridSize.x, gridSize.y);
            var spiralSize = maxVal * maxVal;

            //Search in spiral to find suiting spawn nodes for the squad
            for (var i = 0; i < spiralSize; i++)
            {
                if ((-gridSize.x / 2 <= x && x <= gridSize.x / 2) && (-gridSize.y / 2 <= y && y <= gridSize.y / 2))
                {
                    var currentCoords = new int2(gridSize.x / 2 + x, gridSize.y / 2 + y);
                    if (currentCoords.x >= 0 && currentCoords.x < gridSize.x &&
                        currentCoords.y >= 0 && currentCoords.y < gridSize.y)
                    {
                        var currentNodeIndex = Helpers.CalculateIndex(currentCoords.x, currentCoords.y, gridSize);
                        var startNode = gridNodes[currentNodeIndex];

                        if (squadNodes.IsCreated)
                        {
                            squadNodes.Clear();
                        }

                        PopulateSquadNodes(ref squadNodes, startNode, gridSize, squadSize, gridNodes);

                        if (CanHostSquad(ref squadNodes, squadSize))
                        {
                            //set nodes
                            for (var index = 0; index < squadNodes.Length; index++)
                            {
                                var node = squadNodes[index];
                                var gridNode = gridNodes[node.Index];
                                gridNode.IsFree = false;
                                gridNodes[node.Index] = gridNode;
                            }

                            Debug.Log("Spawned");
                            
                            spawnPosition = new LocalTransform
                            {
                                Position = Helpers.GetPosition(new int2(startNode.X, startNode.Y)),
                                Rotation = quaternion.identity,
                                Scale = 1f,
                            };
                            squadNodes.Dispose();
                            return;
                        }
                    }
                }

                if (x == y || (x < 0 && x == -y) || (x > 0 && x == 1 - y))
                {
                    var temp = dx;
                    dx = -dy;
                    dy = temp;
                }

                x += dx;
                y += dy;
            }

            Debug.Log("Couldn't spawn squad");
            spawnPosition = LocalTransform.FromPosition(0, -100, 0);
            squadNodes.Dispose();
        }

        private void PopulateSquadNodes(ref NativeList<GridNode> results,
            GridNode startNode, 
            int2 gridSize, 
            int2 squadSize, 
            DynamicBuffer<GridNode> gridNodes)
        {
            for (var x = 0; x < squadSize.x; x++)
            {
                for (var y = 0; y < squadSize.y; y++)
                {
                    var currentNodeIndex = Helpers.CalculateIndex(startNode.X + x, startNode.Y + y, gridSize.x);
                    if (currentNodeIndex < 0 || currentNodeIndex >= gridNodes.Length)
                    {
                        continue;
                    }
                    
                    var node = gridNodes[currentNodeIndex];
                    results.Add(node);
                }
            }
        }

        private bool CanHostSquad(ref NativeList<GridNode> hostingNodes, int2 squadSize)
        {
            if (hostingNodes.Length < squadSize.x * squadSize.y)
            {
                return false;
            }
            
            foreach (var hostingNode in hostingNodes)
            {
                if (!hostingNode.IsFree)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool TryGetSquad(DynamicBuffer<SquadElement> squads, int requestedId, out SquadElement squad)
        {
            for (var i = 0; i < squads.Length; i++)
            {
                if (squads[i].SquadId == requestedId)
                {
                    squad = squads[i];
                    return true;
                }
            }

            squad = new SquadElement();
            return false;
        }
    }
}