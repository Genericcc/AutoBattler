using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace DOTS.Battle.Curves
{
    public class AccelerationCurveAuthoring : MonoBehaviour
    {
        public AnimationCurve animationCurve;
        public int numberOfSamples;
        
        private class AccelerationCurveAuthoringBaker : Baker<AccelerationCurveAuthoring>
        {
            public override void Bake(AccelerationCurveAuthoring authoring)
            {
                var blobBuilder = new BlobBuilder(Allocator.Temp);
                ref var discreteCurve = ref blobBuilder.ConstructRoot<DiscreteCurve>();
                var discreteCurveArray = blobBuilder.Allocate(ref discreteCurve.SampledPoints, authoring.numberOfSamples);
                discreteCurve.NumberOfSamples = authoring.numberOfSamples;

                for (var i = 0; i < authoring.numberOfSamples; i++)
                {
                    var samplePoint = (float)i / (authoring.numberOfSamples - 1);
                    var sampleValue = authoring.animationCurve.Evaluate(samplePoint);
                    discreteCurveArray[i] = sampleValue;
                }

                var blobAssetReference = blobBuilder.CreateBlobAssetReference<DiscreteCurve>(Allocator.Persistent);
                var accelerationCurveReference = new AccelerationCurveReference { Value = blobAssetReference };

                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<BlobAssetContainerTag>(entity);
                AddComponent(entity, accelerationCurveReference);
                
                blobBuilder.Dispose();
            }
        }
    }
}