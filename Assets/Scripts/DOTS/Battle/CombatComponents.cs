using System;
using Unity.Entities;
using Unity.Mathematics;

namespace DOTS.Battle
{
    public struct MaxHealth : IComponentData
    {
        public int Value;
    }
    
    public struct CurrentHealth : IComponentData
    {
        public int Value;
    }

    public struct DamageBufferElement : IBufferElementData
    {
        public int Value;
    }
    
    public struct AlreadyDamagedEntity : IBufferElementData
    {
        public Entity Value;
    }

    public struct AttackPrefabs : IComponentData
    {
        public Entity BaseAttackPrefab;
    }

    public struct TempUnitHashAndIndex : IComponentData
    {
        public HashAndIndex HashAndIndex;
    }

    public struct HashAndIndex : IComparable<HashAndIndex>
    {
        public int Hash;
        public int Index;
        
        public int CompareTo(HashAndIndex other)
        {
            return Hash.CompareTo(other.Hash);
        }
    }
    
    public struct DestroyEntityTag : IComponentData {}

    public struct DamageOnHitTrigger : IComponentData
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
    
    public struct CooldownToDestroy : IComponentData
    {
        public float Value;
    }
    
    public struct CountdownToStartTag : IComponentData {}
}














































