using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Common
{
    public class MinionAuthoring : MonoBehaviour
    {
        public float moveSpeed;
            
        private class MinionAuthoringBaker : Baker<MinionAuthoring>
        {
            public override void Bake(MinionAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<MinionTag>(entity);
                AddComponent<NewMinionTag>(entity);
                AddComponent(entity, new CharacterMoveSpeed { Value = authoring.moveSpeed });
                AddComponent<MinionPathIndex>(entity);
                AddBuffer<MinionPathPosition>(entity);
                AddComponent<MobaTeam>(entity);
                AddComponent<URPMaterialPropertyBaseColor>(entity);
            }
        }
    }
}
