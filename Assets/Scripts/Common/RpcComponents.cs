using Unity.NetCode;

namespace Common
{
    public struct MobaTeamRequestRpc : IRpcCommand
    {
        public TeamType Value;
    }

    public struct PlayersRemainingToStartRpc : IRpcCommand
    {
        public int Value;
    }

    public struct GameStartTickRpc : IRpcCommand
    {
        public NetworkTick Value;
    }
    
    
}