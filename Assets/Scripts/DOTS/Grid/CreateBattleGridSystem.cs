using DOTS.Battle;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace DOTS.Grid
{
    public partial struct CreateBattleGridSystem : ISystem
    {
        private NativeArray<TeamBattleGrid> _teamGrids;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<BattleGridDimensions>();
            state.RequireForUpdate<GridSystemData>();

            state.EntityManager.AddComponent<GridSystemData>(state.SystemHandle);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;
            
            var teamBuffer = SystemAPI.GetSingletonBuffer<PlayingTeam>();
            _teamGrids = new NativeArray<TeamBattleGrid>(teamBuffer.Length, Allocator.Persistent);

            var battleGridDimensions = SystemAPI.GetSingleton<BattleGridDimensions>();
            var gridSize = new int2(battleGridDimensions.Width, battleGridDimensions.Height);
            
            for (var i = 0; i < teamBuffer.Length; i++)
            {
                var nodes = new NativeArray<GridNode>(gridSize.x * gridSize.y, Allocator.Persistent);
                                    
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
                    OriginShift = i == 0 ? new int2() : new int2(0, gridSize.y), //TODO zmienić żeby był offset i rotacja?
                    Team = teamBuffer[i].Value, 
                    Nodes = nodes,
                };

                for (var index = 0; index < nodes.Length; index++)
                {
                    var gridNode = nodes[index];
                    teamBattleGrid.Nodes[index] = new GridNode { Index = gridNode.Index, X = gridNode.X, Y = gridNode.Y, IsFree = true};
                }
                
                _teamGrids[i] = teamBattleGrid;
            }
            
            state.EntityManager.SetComponentData(state.SystemHandle, new GridSystemData { TeamBattleGrids = _teamGrids });

            battleGridDimensions.Width = gridSize.x;
            battleGridDimensions.Height = gridSize.y;
        }
    
        public void OnDestroy(ref SystemState state)
        {
            foreach (var teamBattleGrid in _teamGrids)
            {
                // Działa jak trzeba
                // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
                teamBattleGrid.Nodes.Dispose();
            }
            
            _teamGrids.Dispose();
        }
    }
}