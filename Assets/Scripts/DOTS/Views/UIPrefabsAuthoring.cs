using Unity.Entities;
using UnityEngine;

namespace DOTS.Views
{
    public class UIPrefabsAuthoring : MonoBehaviour
    {
        public GameObject healthBarPrefab;
        
        private class UIPrefabsAuthoringBaker : Baker<UIPrefabsAuthoring>
        {
            public override void Bake(UIPrefabsAuthoring authoring)
            {
                var prefabContainerEntity = GetEntity(TransformUsageFlags.None);
                AddComponentObject(prefabContainerEntity, new ViewsPrefabs
                {
                    HealthBar = authoring.healthBarPrefab,
                });
            }
        }
    }
} 