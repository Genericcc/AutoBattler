using Unity.Entities;
using UnityEngine;

namespace DOTS.Battle
{
    public class AttackerAuthoring : MonoBehaviour
    {
        public float attackCooldown;
        public float attackRadius;
        public Vector3 firePointOffset;
        public GameObject attackPrefab;
        
        private class AttackerBaker : Baker<AttackerAuthoring>
        {
            public override void Bake(AttackerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);                
                AddComponent(entity, new AttackRadius
                {
                    Value = authoring.attackRadius
                });
                AddComponent(entity, new AttackProperties
                {
                    AttackPrefab = GetEntity(authoring.attackPrefab, TransformUsageFlags.Dynamic),
                    Cooldown = authoring.attackCooldown,
                    FirePointOffset = authoring.firePointOffset,
                });
                AddComponent(entity, new CurrentCooldown
                {
                    Value = 0f
                });
            }
        }
    }
}