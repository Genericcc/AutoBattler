using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTS
{
    public partial struct SpawnSquadSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SquadElement>();
            state.RequireForUpdate<BattleGridTag>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var availableSquads = SystemAPI.GetSingletonBuffer<SquadElement>();
            var spawnOrders = SystemAPI.GetSingletonBuffer<SquadSpawnOrder>();
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            
            if (spawnOrders.Length <= 0)
            {
                return;
            }

            for (var i = 0; i < spawnOrders.Length; i++)
            {
                if (!TryGetSquad(availableSquads, spawnOrders[i].SquadID, out var squadElement))
                {
                    Debug.Log($"No available squad with ID: {spawnOrders[i].SquadID}!");
                    continue;
                }

                if (!TryFindSpaceForSquad(ref state, squadElement.Size, out var spawnPosition))
                {
                    Debug.Log($"No space found for squad with ID: {spawnOrders[i].SquadID}!");
                    continue;
                }

                var unitShift = new int2(squadElement.Size.x / squadElement.RowUnitCount, squadElement.Size.y / squadElement.ColumnUnitCount);
                
                for (var x = 0; x < squadElement.RowUnitCount; x++)
                {
                    for (var y = 0; y < squadElement.ColumnUnitCount; y++)
                    {
                        var e = ecb.Instantiate(squadElement.Prefab);
                        var shiftedPosition = spawnPosition.Position + new float3(unitShift.x * x, 0, unitShift.y * y);
                        var newPosition = LocalTransform.FromPosition(shiftedPosition);
                        ecb.SetComponent(e, newPosition);
                    }
                }
                    
                Debug.Log("Squad Spawned!");
            }
            
            spawnOrders.Clear();
        }

        private bool TryFindSpaceForSquad(ref SystemState state, int2 squadSize, out LocalTransform spawnPosition)
        {
            var battleGridEntity = SystemAPI.GetSingletonEntity<BattleGridTag>();
            var gridNodes = SystemAPI.GetBuffer<GridNode>(battleGridEntity);
            var gridProperties = SystemAPI.GetComponent<BattleGridProperties>(battleGridEntity);
            
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
                        var startNode = gridNodes[Helpers.CalculateIndex(currentCoords.x, currentCoords.y, gridSize)];
                        var squadNodes = GetNodes(startNode, gridSize, squadSize, gridNodes);

                        if (CanHostSquad(squadNodes, squadSize))
                        {
                            LockNodes(squadNodes, ref gridNodes);
                            
                            spawnPosition = new LocalTransform
                            {
                                Position = Helpers.GetPosition(new int2(startNode.X, startNode.Y)) + new float3(0.5f, 0, 0.5f),
                                Rotation = quaternion.identity,
                                Scale = 1f,
                            };
                            squadNodes.Dispose();
                            return true;
                        }

                        squadNodes.Dispose();
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

            spawnPosition = LocalTransform.FromPosition(0, -100, 0);
            return false;
        }

        private static void LockNodes(NativeList<GridNode> squadNodes, ref DynamicBuffer<GridNode> gridNodes)
        {
            foreach (var node in squadNodes)
            {
                var gridNode = gridNodes[node.Index];
                gridNode.IsFree = false;
                gridNodes[node.Index] = gridNode;
            }
        }

        private NativeList<GridNode> GetNodes(GridNode startNode, 
            int2 gridSize, 
            int2 squadSize, 
            DynamicBuffer<GridNode> gridNodes)
        {
            var results = new NativeList<GridNode>(Allocator.Temp);
            
            for (var x = 0; x < squadSize.x; x++)
            {
                for (var y = 0; y < squadSize.y; y++)
                {
                    var currentCoords = new int2(startNode.X + x, startNode.Y + y);

                    if (currentCoords.x < 0 || currentCoords.x >= gridSize.x ||
                        currentCoords.y < 0 || currentCoords.y >= gridSize.y)
                    {
                        break;
                    }
                    
                    var currentNodeIndex = Helpers.CalculateIndex(currentCoords.x, currentCoords.y, gridSize.x);
                    if (currentNodeIndex < 0 || currentNodeIndex >= gridNodes.Length)
                    {
                        break;
                    }
                    
                    var node = gridNodes[currentNodeIndex];
                    results.Add(node);
                }
            }

            return results;
        }

        private bool CanHostSquad(NativeList<GridNode> hostingNodes, int2 squadSize)
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