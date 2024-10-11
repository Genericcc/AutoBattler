using Unity.Entities;

using UnityEngine;

namespace Common
{
    public class DamageOnTriggerAuthoring : MonoBehaviour
    {
        public int damageOnTrigger;
        private class DamageOnTriggerBaker : Baker<DamageOnTriggerAuthoring>
        {
            public override void Bake(DamageOnTriggerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DamageOnTrigger { Value = authoring.damageOnTrigger });
                AddBuffer<AlreadyDamagedEntity>(entity);
            } 
        }
    }
}