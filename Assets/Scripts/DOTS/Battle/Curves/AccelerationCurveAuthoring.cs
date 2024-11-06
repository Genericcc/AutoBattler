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
                BlobAssetReference<DiscreteCurve> blobAssetReference;
                
                using (var blobBuilder = new BlobBuilder(Allocator.Temp))
                {
                    ref var discreteCurve = ref blobBuilder.ConstructRoot<DiscreteCurve>();
                    
                    var discreteCurveArray = blobBuilder.Allocate(ref discreteCurve.SampledPoints, authoring.numberOfSamples);
                    discreteCurve.NumberOfSamples = authoring.numberOfSamples;

                    for (var i = 0; i < authoring.numberOfSamples; i++)
                    {
                        var samplePoint = (float)i / (authoring.numberOfSamples - 1);
                        var sampleValue = authoring.animationCurve.Evaluate(samplePoint);
                        discreteCurveArray[i] = sampleValue;
                    }

                    blobAssetReference = blobBuilder.CreateBlobAssetReference<DiscreteCurve>(Allocator.Persistent);
                }
                
                AddBlobAsset(ref blobAssetReference, out var hash);
                
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<BlobAssetContainerTag>(entity);
                AddComponent(entity, new AccelerationCurveBlobAsset { Value = blobAssetReference });
            }
        }
    }
}