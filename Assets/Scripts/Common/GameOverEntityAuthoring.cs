using Unity.Entities;
using UnityEngine;

namespace Common
{
    public class GameOverEntityAuthoring : MonoBehaviour
    {
        private class GameOverEntityAuthoringBaker : Baker<GameOverEntityAuthoring>
        {
            public override void Bake(GameOverEntityAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<GameOverTag>(entity);
                AddComponent<WinningTeam>(entity);
            }
        }
    }
}