using Unity.Entities;
using Unity.Physics;
using UnityEngine;

namespace DOTS.Battle.Cameras
{
    public partial class ReadMousePositionSystem : SystemBase
    {
        private CollisionFilter _selectionFilter;

        protected override void OnCreate()
        {
            _selectionFilter = new CollisionFilter
            {
                BelongsTo = 1 << 7, //CameraRaycasts
                CollidesWith = 1 << 0 //GroundPlane
            };
        }

        protected override void OnUpdate()
        {
            var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
            var cameraEntity = SystemAPI.GetSingletonEntity<MainCameraTag>();
            var mainCamera = EntityManager.GetComponentObject<MainCameraEcs>(cameraEntity).Value;

            var inputPosition = Input.mousePosition;
            inputPosition.z = 100f;
            var worldPosition = mainCamera.ScreenToWorldPoint(inputPosition);
 
            var selectionInput = new RaycastInput
            {
                Start = mainCamera.transform.position,
                End = worldPosition,
                Filter = _selectionFilter,
            };

            if (collisionWorld.CastRay(selectionInput, out var closestHit))
            {
                EntityManager.SetComponentData(cameraEntity, new MousePosition
                {
                    Value = closestHit.Position,
                });
            }
        }
    }
}