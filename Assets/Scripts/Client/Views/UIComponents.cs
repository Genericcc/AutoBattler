using Unity.Entities;
using Unity.Mathematics;

using UnityEngine;

namespace Client.Views
{
    //When an Entity is destroyed it kind of still exists, but only with the CleanupComponents 
    //so we still keep out reference to the GameObject
    public class HealthBarUIReference : ICleanupComponentData
    {
        public GameObject Value; 
    }

    public struct HealthBarOffset : IComponentData
    {
        public float3 Value;
    }
    
    public class SkillShotUIReference : ICleanupComponentData
    {
        public GameObject Value; 
    }
}