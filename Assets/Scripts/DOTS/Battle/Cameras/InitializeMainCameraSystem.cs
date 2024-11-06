using Unity.Entities;
using UnityEngine;

namespace DOTS.Battle.Cameras
{
    public partial class InitializeMainCameraSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<MainCameraTag>();
        }

        protected override void OnUpdate()
        {
            Enabled = false;
            var mainCameraEntity = SystemAPI.GetSingletonEntity<MainCameraTag>(); 
            EntityManager.SetComponentData(mainCameraEntity, new MainCameraEcs { Value = Camera.main } );
        }
    }
}