using Unity.Entities;
using UnityEngine;


namespace Common
{
    public class MinionSpawnPropertiesAuthoring : MonoBehaviour
    {
        public float timeBetweenWaves;
        public float timeBetweenMinions;
        public int countToSpawnInWave;
        
        public class MinionSpawnPropertiesBaker : Baker<MinionSpawnPropertiesAuthoring>
        {
            public override void Bake(MinionSpawnPropertiesAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new MinionSpawnProperties
                {
                    TimeBetweenWaves = authoring.timeBetweenWaves,
                    TimeBetweenMinions = authoring.timeBetweenMinions,
                    CountToSpawnInWave = authoring.countToSpawnInWave,
                });
                AddComponent(entity, new MinionSpawnTimers
                {
                    TimeToNextWave = authoring.timeBetweenWaves,
                    TimeToNextMinion = 0f,
                    CountSpawnedInWave = 0,
                });
                
                
            }
        }
    }
}