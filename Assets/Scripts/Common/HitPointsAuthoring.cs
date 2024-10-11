using Client.Views;

using Unity.Entities;

using UnityEngine;

namespace Common
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
                AddComponent(entity, new MaxHitPoints() { Value = authoring.maxHitPoints });
                AddComponent(entity, new CurrentHitPoints { Value = authoring.maxHitPoints });
                AddBuffer<DamageBufferElement>(entity);
                AddBuffer<DamageThisTick>(entity);
                AddComponent(entity, new HealthBarOffset { Value = authoring.healthBarOffset });
            }
        }
    }
}