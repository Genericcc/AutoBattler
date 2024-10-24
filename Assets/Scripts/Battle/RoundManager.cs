using DOTS.Battle;
using DOTS.Rounds;
using Unity.Entities;
using UnityEngine;

namespace Battle
{
    public class RoundManager : MonoBehaviour
    {
        public float cooldownToStart;
        
        private World _world;
        private Entity _entity;

        private void Start()
        {
            _world = World.DefaultGameObjectInjectionWorld;
            
            if (_world.IsCreated && !_world.EntityManager.Exists(_entity))
            {
                _entity = _world.EntityManager.CreateEntity(typeof(RoundManagerTag), typeof(RoundState), typeof(CurrentCooldown));
                _world.EntityManager.SetComponentData(_entity, new CurrentCooldown { Value = cooldownToStart });
                _world.EntityManager.SetName(_entity, "RoundManager");
            }
        }
    }
}