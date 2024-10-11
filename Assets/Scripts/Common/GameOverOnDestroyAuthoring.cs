using Unity.Entities;
using UnityEngine;

namespace Common
{
    public class GameOverOnDestroyAuthoring : MonoBehaviour
    {
        private class GameOverOnDestroyAuthoringBaker : Baker<GameOverOnDestroyAuthoring>
        {
            public override void Bake(GameOverOnDestroyAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<GameOverOnDestroyTag>(entity);
            }
        }
    }
}