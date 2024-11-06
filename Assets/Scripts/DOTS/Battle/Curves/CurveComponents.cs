using Unity.Entities;
using Unity.Mathematics;

namespace DOTS.Battle.Curves
{
    public struct BlobAssetContainerTag : IComponentData {}
    
    public struct AccelerationCurveBlobAsset : IComponentData
    {
        public BlobAssetReference<DiscreteCurve> Value;
        public readonly float GetValueAtTime(float time) => Value.Value.GetValueAtTime(time);
    }
    
    public struct DiscreteCurve
    {
        public BlobArray<float> SampledPoints;
        public int NumberOfSamples;
        
        public float GetValueAtTime(float time)
        {
            var approxSampleIndex = (NumberOfSamples - 1) * time;
            var sampleIndexBelow = (int)math.floor(approxSampleIndex);
            if (sampleIndexBelow >= NumberOfSamples - 1)
            {
                return SampledPoints[NumberOfSamples - 1];
            }
            var indexRemainder = approxSampleIndex - sampleIndexBelow;
            return math.lerp(SampledPoints[sampleIndexBelow], SampledPoints[sampleIndexBelow + 1], indexRemainder);
        }
    }
    
    public struct CurveTimer : IComponentData
    {
        public float Value;
        public float Max;
        public float Normalized => Value / Max;

        public static CurveTimer operator +(CurveTimer curveTimer, float deltaTime)
        {
            return curveTimer.Increment(deltaTime);
        }

        private CurveTimer Increment(float deltaTime)
        {
            Value = math.clamp(Value += deltaTime, 0f, Max);
            return this;
        }

        public void Reset() { Value = 0f; }
    }
}