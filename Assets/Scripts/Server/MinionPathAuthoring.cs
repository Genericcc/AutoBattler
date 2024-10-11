using Common;
using Unity.Entities;
using UnityEngine;

namespace Server
{
    public class MinionPathAuthoring : MonoBehaviour
    {
        public Vector3[] topLanePath;
        public Vector3[] midLanePath;
        public Vector3[] botLanePath;
        
        private class MinionPathAuthoringBaker : Baker<MinionPathAuthoring>
        {
            public override void Bake(MinionPathAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                var topLane = CreateAdditionalEntity(TransformUsageFlags.None, false, "TopLane");
                var midLane = CreateAdditionalEntity(TransformUsageFlags.None, false, "MidLane");
                var botLane = CreateAdditionalEntity(TransformUsageFlags.None, false, "BotLane");

                var topLanePath = AddBuffer<MinionPathPosition>(topLane);
                foreach (var pathPosition in authoring.topLanePath)
                {
                    topLanePath.Add(new MinionPathPosition { Value = pathPosition });
                }
                
                var midLanePath = AddBuffer<MinionPathPosition>(midLane);
                foreach (var pathPosition in authoring.midLanePath)
                {
                    midLanePath.Add(new MinionPathPosition { Value = pathPosition });
                }
                
                var botLanePath = AddBuffer<MinionPathPosition>(botLane);
                foreach (var pathPosition in authoring.botLanePath)
                {
                    botLanePath.Add(new MinionPathPosition { Value = pathPosition });
                }
                
                AddComponent(entity, new MinionPathContainers
                {
                    TopLane = topLane,
                    MidLane = midLane,
                    BotLane = botLane,
                });
            }
        }
    }
}