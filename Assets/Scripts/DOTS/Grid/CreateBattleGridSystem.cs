using DOTS.Battle;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.Grid
{
    public partial struct CreateBattleGridSystem : ISystem
    {
        private NativeArray<GridNode> _gameGridArray;
        private NativeArray<TeamBattleGrid> _gameGrids;

        //[BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<BattleGridDimensions>();
            state.RequireForUpdate<GridSystemData>();

            state.EntityManager.AddComponent<GridSystemData>(state.SystemHandle);
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var teamBuffer = SystemAPI.GetSingletonBuffer<PlayingTeam>();
            var battleGridDimensions = SystemAPI.GetSingleton<BattleGridDimensions>();
            var gridSize = new int2(battleGridDimensions.Width, battleGridDimensions.Height);
            
            _gameGridArray = new NativeArray<GridNode>(gridSize.x * gridSize.y, Allocator.Persistent);
            _gameGrids = new NativeArray<TeamBattleGrid>(teamBuffer.Length, Allocator.Persistent);
            
            Debug.Log($"{teamBuffer.Length} teams");
            
            for (var i = 0; i < teamBuffer.Length; i++)
            {
                var nodes = new NativeArray<GridNode>(gridSize.x * gridSize.y, Allocator.Temp);

                for (var x = 0; x < gridSize.x; x++)
                {
                    for (var y = 0; y < gridSize.y; y++)
                    {
                        var gridNode = new GridNode
                        {
                            X = x,
                            Y = y,

                            Index = Helpers.CalculateIndex(x, y, gridSize),

                            IsFree = true,
                        };

                        nodes[gridNode.Index] = gridNode;
                    }
                }
                
                var teamBattleGrid = new TeamBattleGrid
                {
                    Width = gridSize.x, 
                    Height = gridSize.y, 
                    Team = teamBuffer[i].Value, 
                    Nodes = _gameGridArray
                };

                for (var index = 0; index < nodes.Length; index++)
                {
                    var gridNode = nodes[index];
                    teamBattleGrid.Nodes[index] = new GridNode { Index = gridNode.Index, X = gridNode.X, Y = gridNode.Y, IsFree = true};
                }
                
                _gameGrids[i] = teamBattleGrid;
                
                nodes.Dispose();
            }
            
            state.EntityManager.SetComponentData(state.SystemHandle, new GridSystemData { TeamBattleGrids = _gameGrids });

            battleGridDimensions.Width = gridSize.x;
            battleGridDimensions.Height = gridSize.y;
        }
    
        public void OnDestroy(ref SystemState state)
        {
            _gameGridArray.Dispose();
            _gameGrids.Dispose();
        }
    }
}