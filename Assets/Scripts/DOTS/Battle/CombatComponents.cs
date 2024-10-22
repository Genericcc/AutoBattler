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

    public struct AttackPrefabs : IComponentData
    {
        public Entity BaseAttackPrefab;
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

    public struct ProjectileSpeed : IComponentData
    {
        public float Value;
    }

    public struct EntityTargetingRadius : IComponentData
    {
        public float Value;
    }

    public struct AttackRadius : IComponentData
    {
        public float Value;
    }
    
    public struct TargetEntity : IComponentData
    {
        public Entity Value;
    }
    
    public struct AttackProperties : IComponentData
    {
        public float3 FirePointOffset;
        public float Cooldown;
        public Entity AttackPrefab;
    }
    
    public struct CurrentCooldown : IComponentData
    {
        public float Value;
    }
}














































