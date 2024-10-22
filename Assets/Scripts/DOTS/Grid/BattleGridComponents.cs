using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace DOTS
{
    public struct BattleGridTag : IComponentData {}

    public struct BattleGridProperties : IComponentData
    {
        public int Width;
        public int Height;
    }

    public struct GridNode : IBufferElementData
    {
        public int X;
        public int Y;

        public int Index;

        public bool IsFree;
    }

    // public struct SquadSpawnProperties : IComponentData
    // {
    //     public LocalTransform SpawnTransform;
    //     public NativeArray<GridNode> SpawnNodes;
    // }
}