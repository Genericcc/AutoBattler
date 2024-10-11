using Unity.Entities;

using UnityEngine;

namespace Client
{
    public struct MainCameraTag : IComponentData {}
    
    public class MainCameraEcs : IComponentData
    {
        public Camera Value;
    }
}