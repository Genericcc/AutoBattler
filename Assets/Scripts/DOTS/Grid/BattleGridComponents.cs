using DOTS.Battle;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace DOTS.Grid
{
    public struct BattleGridTag : IComponentData {}

    public struct BattleGridDimensions : IComponentData
    {
        public int Width;
        public int Height;
    }

    public struct GridSystemData : IComponentData
    {
        public NativeArray<TeamBattleGrid> TeamBattleGrids;
    }

    public struct TeamBattleGrid
    {
        public int Width;
        public int Height;        
        public SpawnOffset OriginShift;

        public TeamType Team;
        public NativeArray<GridNode> Nodes;
    }

    public struct GridNode : IComponentData
    {
        public int X;
        public int Y;

        public int Index;

        public bool IsFree;
    }
}