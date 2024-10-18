using Data;
using DOTS;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SquadRecruitButton : Button
    {
        [SerializeField]
        public BaseSquadData _squadData;

        protected override void OnDisable()
        {
            base.OnDisable();
            
            onClick.RemoveAllListeners();
        }

        public void Init(BaseSquadData baseSquadData)
        {
            _squadData = baseSquadData;
            
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
                    });
            }

            entities.Dispose();
        }
    }
}