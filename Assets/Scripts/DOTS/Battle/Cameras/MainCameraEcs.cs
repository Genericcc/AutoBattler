using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS.Battle.Cameras
{
    public struct MainCameraTag : IComponentData {}
    
    public class MainCameraEcs : IComponentData
    {
        public Camera Value;
    }

    public struct MousePosition : IComponentData
    {
        public float3 Value;
    }
}