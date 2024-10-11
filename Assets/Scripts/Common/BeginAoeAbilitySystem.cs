using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

namespace Common
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct BeginAoeAbilitySystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            //Robimy to w BeginSimulation żeby był od razu dobry transform
            //a nie że przed 1 klatkę pojawi się w (0,0,0) i dopiero ustawi na dobrą pozycje
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var networkTime = SystemAPI.GetSingleton<NetworkTime>();

            if (!networkTime.IsFirstTimeFullyPredictingTick)
            {
                return;                
            }

            var currentTick = networkTime.ServerTick;

            foreach (var aoeAspect in SystemAPI.Query<AoeAspect>().WithAll<Simulate>())
            {
                var isOnCooldown = true;
                var currentTargetTicks = new AbilityCooldownTargetTicks();

                //there are cases where the server misses networkTicks, so if we cannot get currentTick that cooldown should end on
                //then we can never set the ability off cooldown
                //so, we check how many Ticks the server missed (and is catching on)
                //and we iterate over them to check if any one is our target cooldown-off Tick
                for (var i = 0u; i < networkTime.SimulationStepBatchSize; i++)
                {
                    var testTick = currentTick;
                    testTick.Subtract(i);

                    if (!aoeAspect.AbilityCooldownTargetTicks.GetDataAtTick(testTick, out currentTargetTicks))
                    {
                        currentTargetTicks.AoeAbilityEndCooldownTick = NetworkTick.Invalid;
                    }

                    if (currentTargetTicks.AoeAbilityEndCooldownTick == NetworkTick.Invalid ||
                        !currentTargetTicks.AoeAbilityEndCooldownTick.IsNewerThan(currentTick))
                    {
                        isOnCooldown = false;
                        break;
                    }
                }

                if (isOnCooldown)
                {
                    continue;
                }
                
                if (aoeAspect.ShouldAttack)
                {
                    var newAoeAbility = ecb.Instantiate(aoeAspect.AbilityPrefab);
                    var abilityTransform = LocalTransform.FromPosition(aoeAspect.AttackPosition);
                    ecb.SetComponent(newAoeAbility, abilityTransform);
                    ecb.SetComponent(newAoeAbility, aoeAspect.Team);
                    
                    if (state.WorldUnmanaged.IsServer())
                    {
                        continue;
                    }

                    var newCooldownTargetTick = currentTick;
                    newCooldownTargetTick.Add(aoeAspect.CooldownTicks);
                    currentTargetTicks.AoeAbilityEndCooldownTick = newCooldownTargetTick;

                    var nextTick = currentTick;
                    nextTick.Add(1u);
                    currentTargetTicks.Tick = nextTick;
                    
                    aoeAspect.AbilityCooldownTargetTicks.AddCommandData(currentTargetTicks);                    
                }
            }

        }
    }
}