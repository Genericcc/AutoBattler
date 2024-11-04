using Unity.Entities;

namespace DOTS.Battle
{
    public struct Team : IComponentData
    {
        public TeamType Value;
    }
    
    public struct PlayingTeam : IBufferElementData
    {
        public TeamType Value;
    }
    
    public struct MoveSpeed : IComponentData
    {
        public float Value;
    }
}