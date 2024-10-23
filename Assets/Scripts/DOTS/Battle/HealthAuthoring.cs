using DOTS.Views;
using Unity.Entities;
using UnityEngine;

namespace DOTS.Battle
{
    public class HealthAuthoring : MonoBehaviour
    {
        public int maxHealth;
        public Vector3 healthBarOffset;
        private class HealthAuthoringBaker : Baker<HealthAuthoring>
        {
            public override void Bake(HealthAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MaxHealth { Value = authoring.maxHealth });
                AddComponent(entity, new CurrentHealth { Value = authoring.maxHealth });
                AddBuffer<DamageBufferElement>(entity);
                AddComponent(entity, new HealthBarOffset { Value = authoring.healthBarOffset });
            }
        }
    }
}