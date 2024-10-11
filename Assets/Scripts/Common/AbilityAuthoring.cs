using Unity.Entities;
using Unity.NetCode;

using UnityEngine;

namespace Common
{
    public class AbilityAuthoring : MonoBehaviour
    {
        public GameObject aoeAbilityPrefab;
        public GameObject skillShotAbilityPrefab;
        public float aoeAbilityCooldown;
        public float skillShotAbilityCooldown;

        public NetCodeConfig netCodeConfig;
        private int SimulationTickRate => netCodeConfig.ClientServerTickRate.SimulationTickRate;
        
        private class AbilityBaker : Baker<AbilityAuthoring>
        {
            public override void Bake(AbilityAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new AbilityPrefabs
                {
                    AoeAbility = GetEntity(authoring.aoeAbilityPrefab, TransformUsageFlags.Dynamic),
                    SkillShotAbility = GetEntity(authoring.skillShotAbilityPrefab, TransformUsageFlags.Dynamic),
                });
                AddComponent(entity, new AbilityCooldownTicks
                {
                    AoeAbilityCooldownTicks = (uint)(authoring.aoeAbilityCooldown * authoring.SimulationTickRate),
                    SkillShotAbilityCooldownTicks = (uint)(authoring.skillShotAbilityCooldown * authoring.SimulationTickRate),
                });
                AddBuffer<AbilityCooldownTargetTicks>(entity);
            }
        }
    }
}