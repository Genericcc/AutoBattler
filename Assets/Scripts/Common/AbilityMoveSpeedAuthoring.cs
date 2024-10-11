using Unity.Entities;

using UnityEngine;

namespace Common
{
    public class AbilityMoveSpeedAuthoring : MonoBehaviour
    {
        public float abilityMoveSpeed;
        private class AbilityMoveSpeedAuthoringBaker : Baker<AbilityMoveSpeedAuthoring>
        {
            public override void Bake(AbilityMoveSpeedAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new AbilityMoveSpeed { Value = authoring.abilityMoveSpeed } );
            }
        }
    }
}