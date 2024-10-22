using Unity.Entities;
using UnityEngine;

namespace DOTS.Battle
{
    public class MoveSpeedAuthoring : MonoBehaviour
    {
        public float moveSpeed;
        private class MoveSpeedAuthoringBaker : Baker<MoveSpeedAuthoring>
        {
            public override void Bake(MoveSpeedAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MoveSpeed { Value = authoring.moveSpeed });
            }
        }
    }
}