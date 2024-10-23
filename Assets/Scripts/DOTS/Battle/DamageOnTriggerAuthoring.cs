using Unity.Entities;
using UnityEngine;

namespace DOTS.Battle
{
    public class DamageOnTriggerAuthoring : MonoBehaviour
    {
        public int damageOnHitTrigger;
        private class DamageOnTriggerBaker : Baker<DamageOnTriggerAuthoring>
        {
            public override void Bake(DamageOnTriggerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DamageOnHitTrigger { Value = authoring.damageOnHitTrigger });
            } 
        }
    }
}