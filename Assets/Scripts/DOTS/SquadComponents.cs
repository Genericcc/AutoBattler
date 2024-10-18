using Unity.Entities;
using Unity.Mathematics;

namespace DOTS
{
    public struct SquadRecruitmentManagerTag : IComponentData {}

    public struct SquadSpawnOrder : IBufferElementData
    {
        public int SquadID;
    }
    
    public struct SquadElement : IBufferElementData
    {
        public int SquadId;
        public Entity Prefab;
        public int2 Size;
    }

    // public struct SquadSpawnInput : IComponentData
    // {
    //     public bool SpawnWarriorSquad;
    // }
}