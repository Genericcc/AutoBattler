using DOTS.Views;
using Unity.Entities;
using UnityEngine;

namespace DOTS.Battle
{
    public class HitPointsAuthoring : MonoBehaviour
    {
        public int maxHitPoints;
        public Vector3 healthBarOffset;
        private class HitPointsAuthoringBaker : Baker<HitPointsAuthoring>
        {
            public override void Bake(HitPointsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MaxHealth { Value = authoring.maxHitPoints });
                AddComponent(entity, new CurrentHealth { Value = authoring.maxHitPoints });
                AddBuffer<DamageBufferElement>(entity);
                AddComponent(entity, new HealthBarOffset { Value = authoring.healthBarOffset });
            }
        }
    }
}