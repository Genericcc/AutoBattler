using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS.Battle
{
    public struct MaxHitPoints : IComponentData
    {
        public int Value;
    }
    public struct CurrentHitPoints : IComponentData
    {
        public int Value;
    }

    public struct DamageBufferElement : IBufferElementData
    {
        public int Value;
    }

    // [GhostComponent(PrefabType = GhostPrefabType.AllPredicted, OwnerSendType = SendToOwnerType.SendToNonOwner)]
    // public struct DamageThisTick : ICommandData
    // {
    //     public NetworkTick Tick { get; set; }
    //     public int Value;
    // }

    public struct AbilityPrefabs : IComponentData
    {
        public Entity AoeAbility;
        public Entity SkillShotAbility;
    }

    public struct DestroyOnTimer : IComponentData
    {
        public float Value;
    }
    
    public struct DestroyEntityTag : IComponentData {}

    public struct DamageOnTrigger : IComponentData
    {
        public int Value;
    }
    
    public struct AlreadyDamagedEntity : IBufferElementData
    {
        public Entity Value;
    }

    public struct AbilityMoveSpeed : IComponentData
    {
        public float Value;
    }

    public struct TargetingRadius : IComponentData
    {
        public float Value;
    }
    
    public struct TargetEntity : IComponentData
    {
        public Entity Target;
        public LocalTransform TargetTransform;
    }
    
    public struct NpcAttackProperties : IComponentData
    {
        public float3 FirePointOffset;
        public uint CooldownTickCount;
        public Entity AttackPrefab;
    }
}














































