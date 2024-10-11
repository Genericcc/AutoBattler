using Unity.Entities;

using UnityEngine;

namespace Common
{
    public class MobaPrefabsAuthoring : MonoBehaviour
    {
        [Header("Entities")]
        public GameObject champion;
        public GameObject minion;
        public GameObject gameOverEntity;

        [Header("GameObjects")]
        public GameObject healthBarPrefab;
        public GameObject skillShotPrefab;
        
        private class MobaPrefabsAuthoringBaker : Baker<MobaPrefabsAuthoring>
        {
            public override void Bake(MobaPrefabsAuthoring authoring)
            {
                var prefabContainerEntity = GetEntity(TransformUsageFlags.None);
                AddComponent(prefabContainerEntity, new MobaPrefabs
                {
                    Champion = GetEntity(authoring.champion, TransformUsageFlags.Dynamic),
                    Minion = GetEntity(authoring.minion, TransformUsageFlags.Dynamic),
                    GameOverEntity = GetEntity(authoring.gameOverEntity, TransformUsageFlags.None),
                });
                
                AddComponentObject(prefabContainerEntity, new UIPrefabs
                {
                    HealthBar = authoring.healthBarPrefab,                    
                    SkillShot = authoring.skillShotPrefab,
                });
            }
        }
    }
}