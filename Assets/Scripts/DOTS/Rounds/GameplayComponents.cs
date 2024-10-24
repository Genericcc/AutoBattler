using Unity.Entities;

namespace DOTS.Rounds
{
    public struct RoundManagerTag : IComponentData {}

    public struct RoundState : IComponentData
    {
        public RoundStateType RoundStateType;
    }

    public enum RoundStateType
    {
        Prepare = 0,
        Playing = 1,
        Finish = 2,
    }
}