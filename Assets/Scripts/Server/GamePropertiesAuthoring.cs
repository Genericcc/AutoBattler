using Unity.Entities;
using UnityEngine;

namespace Server
{
    public class GamePropertiesAuthoring : MonoBehaviour
    {
        public int maxPlayersPerTeam;
        public int minPlayersToStartGame;
        public int countdownTime;
        public Vector3[] spawnOffsets;
         
        private class GamePropertiesAuthoringBaker : Baker<GamePropertiesAuthoring>
        {
            public override void Bake(GamePropertiesAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new GameStartProperties
                {
                    MaxPlayersPerTeam = authoring.maxPlayersPerTeam,
                    MinPlayersToStartGame = authoring.minPlayersToStartGame,
                    CountdownTime = authoring.countdownTime,
                });
                
                AddComponent<TeamPlayerCounter>(entity);
                
                var spawnOffsetBuffer = AddBuffer<SpawnOffset>(entity);
                foreach (var spawnOffset in authoring.spawnOffsets)
                {
                    spawnOffsetBuffer.Add(new SpawnOffset { Value = spawnOffset });
                }
            }
        }
    }
}