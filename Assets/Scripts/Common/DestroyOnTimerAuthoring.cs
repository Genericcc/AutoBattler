using Unity.Entities;

using UnityEngine;

namespace Common
{
    public class DestroyOnTimerAuthoring : MonoBehaviour
    {
        public float destroyOnTimer;
        private class DestroyOnTimerAuthoringBaker : Baker<DestroyOnTimerAuthoring>
        {
            public override void Bake(DestroyOnTimerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DestroyOnTimer{ Value = authoring.destroyOnTimer } );
            }
        }
    }
}