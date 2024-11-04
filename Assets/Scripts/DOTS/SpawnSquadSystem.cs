using DOTS.Battle;
using DOTS.Grid;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace DOTS
{
    public partial struct SpawnSquadSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GridSystemData>();
            state.RequireForUpdate<BattleGridDimensions>();
            state.RequireForUpdate<SquadData>();
            //state.RequireForUpdate<BattleGridTag>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var spawnOrders = SystemAPI.GetSingletonBuffer<SquadSpawnOrder>();
            
            if (spawnOrders.Length <= 0)
            {
                return;
            }
            
            var availableSquads = SystemAPI.GetSingletonBuffer<SquadData>();
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var squadSpawnOrder in spawnOrders)
            {
                if (!TryGetSquad(availableSquads, squadSpawnOrder.SquadID, out var squadElement))
                {
                    Debug.Log($"No available squad with ID: {squadSpawnOrder.SquadID}!");
                    continue;
                }

                if (!TryFindNodesForSquad(ref state, squadElement.Size, squadSpawnOrder.TeamType, out var spawnNodes))
                {
                    Debug.Log($"No space found for squad with ID: {squadSpawnOrder.SquadID}!");
                    spawnNodes.Dispose();
                    continue;
                }
                
                LockNodes(ref state, spawnNodes, squadSpawnOrder.TeamType);

                var spawnPosition = GetSpawnPosition(spawnNodes);

                var unitShift = new int2(squadElement.Size.x / squadElement.RowUnitCount, squadElement.Size.y / squadElement.ColumnUnitCount);
                
                for (var x = 0; x < squadElement.RowUnitCount; x++)
                {
                    for (var y = 0; y < squadElement.ColumnUnitCount; y++)
                    {
                        var squadUnit = ecb.Instantiate(squadElement.Prefab);
                        var shiftedPosition = spawnPosition.Position + new float3(unitShift.x * x, 0, unitShift.y * y);
                        var newPosition = LocalTransform.FromPosition(shiftedPosition);
                        var teamColor = squadSpawnOrder.TeamType switch
                        {
                            TeamType.Blue => new float4(0, 0, 1, 1),
                            TeamType.Red => new float4(1, 0, 0, 1),
                            _ => new float4(0),
                        };
                        
                        ecb.SetComponent(squadUnit, newPosition);
                        ecb.SetComponent(squadUnit, new Team { Value = squadSpawnOrder.TeamType});
                        ecb.SetComponent(squadUnit, new URPMaterialPropertyBaseColor { Value = teamColor, });
                    }
                }
                
                spawnNodes.Dispose();
            }
            
            spawnOrders.Clear();
        }

        [BurstCompile]
        private bool TryFindNodesForSquad(ref SystemState state, int2 squadSize, TeamType squadTeam, out NativeList<GridNode> squadNodes)
        {
            //var battleGridEntity = SystemAPI.GetSingletonEntity<BattleGridTag>();
            // var gridNodes = SystemAPI.GetBuffer<GridNode>(battleGridEntity);
            var gridProperties = SystemAPI.GetSingleton<BattleGridDimensions>();
            var gridSystemData = SystemAPI.GetSingleton<GridSystemData>();            
            
            var gridNodes = new NativeArray<GridNode>();
            squadNodes = new NativeList<GridNode>();
            
            foreach (var teamBattleGrid in gridSystemData.TeamBattleGrids)
            {
                if (teamBattleGrid.Team == squadTeam)
                {
                    gridNodes = teamBattleGrid.Nodes;
                    break;
                }
            }

            if (gridNodes.Length <= 0)
            {
                Debug.Log($"Coś się spierdoliło, nie ma gridu dla teamu: {squadTeam}");
                gridNodes.Dispose();
                return false;
            }

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
                        squadNodes = GetNodes(startNode, gridSize, squadSize, gridNodes);

                        if (CanHostSquad(squadNodes, squadSize))
                        {
                            return true;
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
            
            return false;
        }

        //TODO test and fix if needed
        [BurstCompile]
        private LocalTransform GetSpawnPosition(NativeList<GridNode> squadNodes)
        {
            var startNode = squadNodes[0];
            
            return new LocalTransform
            {
                Position = Helpers.GetPosition(new int2(startNode.X, startNode.Y)) + new float3(0.5f, 0, 0.5f),
                Rotation = quaternion.identity,
                Scale = 1f,
            };
        }

        [BurstCompile]
        private void LockNodes(ref SystemState state, in NativeList<GridNode> squadNodes, TeamType teamType)
        {
            var gridSystemData = SystemAPI.GetSingleton<GridSystemData>();
            var gridNodes = new NativeArray<GridNode>();
            
            foreach (var teamBattleGrid in gridSystemData.TeamBattleGrids)
            {
                if (teamBattleGrid.Team == teamType)
                {
                    gridNodes = teamBattleGrid.Nodes;
                }
            }

            if (gridNodes.Length <= 0)
            {
                Debug.Log($"Coś się spierdoliło, Lock nieudany");
            }
            
            foreach (var node in squadNodes)
            {
                var gridNode = gridNodes[node.Index];
                gridNode.IsFree = false;
                gridNodes[node.Index] = gridNode;
            }
        }

        [BurstCompile]
        private NativeList<GridNode> GetNodes(GridNode startNode, 
            int2 gridSize, 
            int2 squadSize, 
            NativeArray<GridNode> gridNodes)
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

        [BurstCompile]
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

        [BurstCompile]
        private static bool TryGetSquad(in DynamicBuffer<SquadData> squads, int requestedId, out SquadData result)
        {
            for (var i = 0; i < squads.Length; i++)
            {
                if (squads[i].SquadId == requestedId)
                {
                    result = squads[i];
                    return true;
                }
            }

            result = new SquadData();
            return false;
        }
    }
}