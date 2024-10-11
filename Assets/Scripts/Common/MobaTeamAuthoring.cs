using Unity.Entities;

using UnityEngine;

namespace Common
{
    public class MobaTeamAuthoring : MonoBehaviour
    {
        public TeamType team;
        private class MobaTeamAuthoringBaker : Baker<MobaTeamAuthoring>
        {
            public override void Bake(MobaTeamAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(e, new MobaTeam { Value = authoring.team} );
            }
        }
    }
}