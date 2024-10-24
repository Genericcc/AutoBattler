using System;
using DOTS.Battle;
using Unity.Collections;
using Unity.Entities;

namespace DOTS.Rounds
{
    public partial class CountdownToGameStartSystem : SystemBase
    {
        public Action<float> OnUpdateCountdownText;
        public Action OnCountdownEnd;

        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (roundState, cooldownToStart, entity) 
                     in SystemAPI.Query<RefRW<RoundState>, RefRW<CurrentCooldown>>().WithEntityAccess())
            {
                if (cooldownToStart.ValueRO.Value > 0)
                {
                    OnUpdateCountdownText?.Invoke(cooldownToStart.ValueRO.Value);
                    continue;
                }

                roundState.ValueRW.RoundStateType = RoundStateType.Playing;
                cooldownToStart.ValueRW.Value = 60f;
                
                OnCountdownEnd?.Invoke();
            }
            
            ecb.Playback(EntityManager);
        }
    }
}