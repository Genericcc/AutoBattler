using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

namespace Common
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
    public partial struct CalculateFrameDamageSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var currentTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;

            foreach (var (damageBuffer, damageThisTickBuffer) 
                     in SystemAPI.Query<DynamicBuffer<DamageBufferElement>, DynamicBuffer<DamageThisTick>>()
                                 .WithAll<Simulate>())
            {
                if (damageBuffer.IsEmpty)
                {
                    damageThisTickBuffer.AddCommandData(new DamageThisTick { Tick = currentTick, Value = 0 } );
                }
                else
                {
                    var totalDamage = 0;

                    //there could have already been damage added on this network tick (since there can be more client ticks than server's)
                    //so we need to get the already added damage before we add our new damage
                    //and then on network tick it will all be dealt
                    if (damageThisTickBuffer.GetDataAtTick(currentTick, out var damageThisTick))
                    {
                        totalDamage += damageThisTick.Value;
                    }

                    foreach (var damageElement in damageBuffer)
                    {
                        totalDamage += damageElement.Value;
                    }
                    
                    damageThisTickBuffer.AddCommandData(new DamageThisTick { Tick = currentTick, Value = totalDamage } );
                    damageBuffer.Clear();
                }
            }
        }
    }
}