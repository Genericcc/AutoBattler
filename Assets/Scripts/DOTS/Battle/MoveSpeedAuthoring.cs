using DOTS.Battle.Curves;
using Unity.Entities;
using UnityEngine;

namespace DOTS.Battle
{
    public class MoveSpeedAuthoring : MonoBehaviour
    {
        public float moveSpeed;
        public float accelerationTime;
        
        private class MoveSpeedAuthoringBaker : Baker<MoveSpeedAuthoring>
        {
            public override void Bake(MoveSpeedAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MoveSpeed { MaxSpeed = authoring.moveSpeed, CurrentSpeedModifier = 0f });
                AddComponent(entity, new CurveTimer { Max = authoring.accelerationTime, Value = 0f });
            }
        }
    }
}