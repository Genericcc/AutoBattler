using System;
using System.Collections.Generic;
using Data;
using DOTS;
using Sirenix.OdinInspector;
using UI;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Battle
{
    public class SquadRecruitmentManager : MonoBehaviour
    {
        [Title("References")] //==========
        
        [SerializeField] 
        private SquadRecruitmentView squadRecruitmentView;
        
        [FormerlySerializedAs("testSquadsData")]
        [FormerlySerializedAs("testSquadData")]
        [FormerlySerializedAs("availableSquads")]
        [Title("Data")] //================
        
        [SerializeField] 
        private TestSquadsRegister testSquadsRegister;
        
        private World _world;
        private Entity _entity;

        private void Start()
        {
            _world = World.DefaultGameObjectInjectionWorld;
            
            if (_world.IsCreated && !_world.EntityManager.Exists(_entity))
            {
                _entity = _world.EntityManager.CreateEntity(typeof(SquadRecruitmentManagerTag));
                _world.EntityManager.AddBuffer<SquadSpawnOrder>(_entity);
            }

            squadRecruitmentView.Init(testSquadsRegister.availableSquads);
        }
    }
}