using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace DOTS.Battle
{
    public class TeamAuthoring : MonoBehaviour
    {
        public TeamType team;
        private class SquadUnitAuthoringBaker : Baker<TeamAuthoring>
        {
            public override void Bake(TeamAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Team { Value = authoring.team });
                AddComponent<URPMaterialPropertyBaseColor>(entity);
            }
        }
    }
}