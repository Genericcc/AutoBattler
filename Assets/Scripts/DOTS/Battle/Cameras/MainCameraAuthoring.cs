﻿using Unity.Entities;
using UnityEngine;

namespace DOTS.Battle.Cameras
{
    public class MainCameraAuthoring : MonoBehaviour
    {
        public class MainCameraBaker : Baker<MainCameraAuthoring>
        {
            public override void Bake(MainCameraAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponentObject(entity, new MainCameraEcs());
                AddComponent<MainCameraTag>(entity);
                AddComponent<MousePosition>(entity);
            }
        }
    }
}