using DOTS.Battle;
using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public struct SquadRecruitmentManagerTag : IComponentData {}

    public struct SquadSpawnOrder : IBufferElementData
    {
        public int SquadID;
        public TeamType TeamType;
    }
    
    public struct SquadData : IBufferElementData
    {
        public int SquadId;
        public Entity Prefab;
        public int2 Size;
        public int RowUnitCount;
        public int ColumnUnitCount;
    }
}