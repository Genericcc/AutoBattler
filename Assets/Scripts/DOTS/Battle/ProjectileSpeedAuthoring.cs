using Unity.Entities;
using UnityEngine;

namespace DOTS.Battle
{
    public class ProjectileSpeedAuthoring : MonoBehaviour
    {
        public float projectileSpeed;
        private class ProjectileSpeedAuthoringBaker : Baker<ProjectileSpeedAuthoring>
        {
            public override void Bake(ProjectileSpeedAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ProjectileSpeed { Value = authoring.projectileSpeed } );
            }
        }
    }
}