using Common;

using Unity.Entities;

namespace Client
{
    public struct ClientTeamRequest : IComponentData
    {
        public TeamType Value;
    }
}