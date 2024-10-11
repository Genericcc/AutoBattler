using Unity.Entities;
using Unity.NetCode;

using UnityEngine;

namespace Common
{
    public class NpcAttackAuthoring : MonoBehaviour
    {
        public float attackCooldown;
        public float npcTargetRadius;
        public Vector3 firePointOffset;
        public GameObject attackPrefab;

        public NetCodeConfig netCodeConfig;
        private int SimulationTickRate => netCodeConfig.ClientServerTickRate.SimulationTickRate;
        
        private class NpcAttackBaker : Baker<NpcAttackAuthoring>
        {
            public override void Bake(NpcAttackAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);                
                AddComponent(entity, new NpcTargetRadius { Value = authoring.npcTargetRadius });
                AddComponent(entity, new NpcAttackProperties
                {
                    AttackPrefab = GetEntity(authoring.attackPrefab, TransformUsageFlags.Dynamic),
                    CooldownTickCount = (uint)(authoring.attackCooldown * authoring.SimulationTickRate),
                    FirePointOffset = authoring.firePointOffset,
                });
                AddComponent<NpcTargetEntity>(entity);
                AddBuffer<NpcAttackCooldownCommandData>(entity);
            }
        }
    }
}