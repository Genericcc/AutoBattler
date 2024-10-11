using Common;

using Unity.Entities;
using Unity.NetCode;

namespace Client.Views
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct AbilityCooldownUISystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var currentTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
            var abilityUIController = AbilityCooldownUIController.Instance;

            foreach (var (cooldownTargetTicks, abilityCooldownTicks) 
                     in SystemAPI.Query<DynamicBuffer<AbilityCooldownTargetTicks>, AbilityCooldownTicks>())
            {
                if (!cooldownTargetTicks.GetDataAtTick(currentTick, out var currentTargetTicks))
                {
                    currentTargetTicks.AoeAbilityEndCooldownTick = NetworkTick.Invalid;
                    currentTargetTicks.SkillShotAbilityEndCooldownTick = NetworkTick.Invalid;
                }
                
                //Update aoe ui
                if (currentTargetTicks.AoeAbilityEndCooldownTick == NetworkTick.Invalid ||
                    currentTick.IsNewerThan(currentTargetTicks.AoeAbilityEndCooldownTick))
                {
                    abilityUIController.UpdateAoeMask(0f);
                }
                else
                {
                    var aoeRemainingTickCount = currentTargetTicks.AoeAbilityEndCooldownTick.TickIndexForValidTick -
                                                currentTick.TickIndexForValidTick;
                    var fillAmount = (float)aoeRemainingTickCount / abilityCooldownTicks.AoeAbilityCooldownTicks;
                    abilityUIController.UpdateAoeMask(fillAmount);
                }

                //Update skillshot ui
                if (currentTargetTicks.SkillShotAbilityEndCooldownTick == NetworkTick.Invalid ||
                    currentTick.IsNewerThan(currentTargetTicks.SkillShotAbilityEndCooldownTick))
                {
                    abilityUIController.UpdateSkillShotMask(0f);
                }
                else
                {
                    var skillShotRemainingTickCount = currentTargetTicks.SkillShotAbilityEndCooldownTick.TickIndexForValidTick -
                                                      currentTick.TickIndexForValidTick;
                    var fillAmount = (float)skillShotRemainingTickCount / abilityCooldownTicks.SkillShotAbilityCooldownTicks;
                    abilityUIController.UpdateSkillShotMask(fillAmount);
                }
                
            }
            
        }
    }
}