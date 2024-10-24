using Data;
using DOTS;
using DOTS.Battle;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.UI;

namespace UI
{
    public class SquadRecruitButton : Button
    {
        [ShowInInspector]
        private BaseSquadData _squadData;

        private TeamType _teamType;

        protected override void OnDisable()
        {
            base.OnDisable();
            
            onClick.RemoveAllListeners();
        }

        public void Init(BaseSquadData baseSquadData, TeamType teamType)
        {
            _squadData = baseSquadData;
            _teamType = teamType;
            
            onClick.AddListener(SpawnSquad);
        }
        
        public void SpawnSquad()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entityQuery = entityManager.CreateEntityQuery(typeof(SquadRecruitmentManagerTag));
            var entities = entityQuery.ToEntityArray(Allocator.TempJob);

            foreach (var entity in entities)
            {
                entityManager
                    .GetBuffer<SquadSpawnOrder>(entity)
                    .Add(new SquadSpawnOrder 
                    { 
                        SquadID = _squadData.squadDataID,
                        TeamType = _teamType,
                    });
            }

            entities.Dispose();
        }
    }
}