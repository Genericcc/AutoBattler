using Unity.Entities;
using UnityEngine;

namespace DOTS.Battle
{
    public class TargetingAuthoring : MonoBehaviour
    {
        public float targetingRadius;
        private class TargetingAuthoringBaker : Baker<TargetingAuthoring>
        {
            public override void Bake(TargetingAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EntityTargetingRadius { Value = authoring.targetingRadius });
                AddComponent<TargetEntity>(entity);
            }
        }
    }
}