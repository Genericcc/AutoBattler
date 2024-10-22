using Battle;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS
{
    public partial struct CreateBattleGridSystem : ISystem
    {
        public NativeArray<GridNode> GridNodes;
        public int2 GridSize;
        
        //[BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<BattleGridProperties>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var battleGridProperties = SystemAPI.GetSingleton<BattleGridProperties>();
            GridSize = new int2(battleGridProperties.Width, battleGridProperties.Height);

            if (GridNodes.IsCreated)
            {
                GridNodes.Dispose();
            }
            
            GridNodes = new NativeArray<GridNode>(GridSize.x * GridSize.y, Allocator.Temp);
            
            for (var x = 0; x < GridSize.x; x++)
            {
                for (var y = 0; y < GridSize.y; y++) 
                {
                    var gridNode = new GridNode
                    {
                        X = x,
                        Y = y,
                        
                        Index = Helpers.CalculateIndex(x, y, GridSize),
                        
                        IsFree = true,
                    };

                    GridNodes[gridNode.Index] = gridNode;
                }
            }

            var entity = ecb.CreateEntity(); 
            ecb.AddComponent<BattleGridTag>(entity);
            ecb.AddComponent(entity, new BattleGridProperties
            {
                Width = GridSize.x,
                Height = GridSize.y,
            });
            
            ecb.AddBuffer<GridNode>(entity);
            foreach (var gridNode in GridNodes)
            {
                ecb.AppendToBuffer(entity, gridNode);
            }
        }
        
        public LocalTransform GetMiddlePosition(int2 squadDataSize)
        {
            var middlePos = GridNodes[Helpers.CalculateIndex(GridSize.x / 2, GridSize.y / 2, GridSize)];
            
            var position = Helpers.GetPosition(new int2(middlePos.X, middlePos.Y));
            var localTransform = LocalTransform.FromPositionRotation(position, quaternion.identity);

            return localTransform;
        }
    }
}