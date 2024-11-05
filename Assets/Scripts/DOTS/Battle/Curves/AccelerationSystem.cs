using DOTS.Rounds;
using Unity.Burst;
using Unity.Entities;

namespace DOTS.Battle.Curves
{
    public partial struct AccelerationSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var roundState in SystemAPI.Query<RoundState>())
            {
                if (roundState.RoundStateType != RoundStateType.Playing)
                {
                    return;
                }
            }
            
            var deltaTime = SystemAPI.Time.DeltaTime;
            
            foreach(var (curveTimer, moveSpeed, accelerationCurve) 
                    in SystemAPI.Query<RefRW<CurveTimer>, RefRW<MoveSpeed>, AccelerationCurveReference>())
            {
                if (curveTimer.ValueRO.Value < curveTimer.ValueRO.Max)
                {
                    curveTimer.ValueRW += deltaTime;
                    moveSpeed.ValueRW.CurrentSpeedModifier = accelerationCurve.GetValueAtTime(curveTimer.ValueRO.Normalized);
                }
            }
        }
    }
}