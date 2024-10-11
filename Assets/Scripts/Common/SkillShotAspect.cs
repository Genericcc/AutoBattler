using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Common
{
    public readonly partial struct SkillShotAspect : IAspect
    {
        public readonly Entity ChampionEntity;
        
        private readonly RefRO<AbilityInput> _abilityInput;
        private readonly RefRO<AbilityPrefabs> _abilityPrefabs;
        private readonly RefRO<MobaTeam> _mobaTeam;
        private readonly RefRO<LocalTransform> _localTransform;
        private readonly RefRO<AbilityCooldownTicks> _abilityCooldownTicks;
        private readonly DynamicBuffer<AbilityCooldownTargetTicks> _abilityCooldownTargetTicks;        
        
        private readonly RefRO<AimInput> _aimInput;

        
        public bool BeginAttack => _abilityInput.ValueRO.SkillShotAbility.IsSet;
        public bool ConfirmAttack => _abilityInput.ValueRO.ConfirmSkillShotAbility.IsSet;
        public Entity AbilityPrefab => _abilityPrefabs.ValueRO.SkillShotAbility;
        public MobaTeam Team => _mobaTeam.ValueRO;
        public uint CooldownTicks => _abilityCooldownTicks.ValueRO.SkillShotAbilityCooldownTicks;
        public DynamicBuffer<AbilityCooldownTargetTicks> AbilityCooldownTargetTicks => _abilityCooldownTargetTicks;
        
        public float3 AttackPosition => _localTransform.ValueRO.Position;
        public LocalTransform SpawnPosition => LocalTransform.FromPositionRotation(
            _localTransform.ValueRO.Position,
            quaternion.LookRotationSafe(_aimInput.ValueRO.Value, math.up()));



    }
}