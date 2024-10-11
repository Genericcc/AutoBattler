using Client.Views;

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

using UnityEngine;

namespace Common
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct BeginSkillShotSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();
            if (!networkTime.IsFirstTimeFullyPredictingTick) { return; }
            var currentTick = networkTime.ServerTick;
            var isServer = state.WorldUnmanaged.IsServer();

            //If ability button was pressed, add AimSkillShotTag, so we can Query it later
            //Also instantiate visual
            foreach (var skillShot in SystemAPI.Query<SkillShotAspect>().WithAll<Simulate>().WithNone<AimSkillShotTag>())
            {
                var isOnCooldown = true;

                for (var i = 0u; i < networkTime.SimulationStepBatchSize; i++)
                {
                    var testTick = currentTick;
                    testTick.Subtract(i);

                    if (!skillShot.AbilityCooldownTargetTicks.GetDataAtTick(currentTick, out var currentTargetTicks))
                    {
                        currentTargetTicks.SkillShotAbilityEndCooldownTick = NetworkTick.Invalid;
                    }

                    if (currentTargetTicks.SkillShotAbilityEndCooldownTick == NetworkTick.Invalid ||
                        !currentTargetTicks.SkillShotAbilityEndCooldownTick.IsNewerThan(currentTick))
                    {
                        isOnCooldown = false;
                        break;
                    }
                }
                
                if (isOnCooldown) continue;
                if (!skillShot.BeginAttack) continue;
                
                ecb.AddComponent<AimSkillShotTag>(skillShot.ChampionEntity);
                
                //Run this only on the Client using the skillShot
                if (isServer || !SystemAPI.HasComponent<OwnerChampTag>(skillShot.ChampionEntity)) continue;
                
                var skillShoutUIPrefab = SystemAPI.ManagedAPI.GetSingleton<UIPrefabs>().SkillShot;
                var newSkillShotUI = Object.Instantiate(skillShoutUIPrefab, skillShot.AttackPosition, Quaternion.identity);
                ecb.AddComponent(skillShot.ChampionEntity, new SkillShotUIReference { Value = newSkillShotUI });
            }

            //If skillshot was confirmed, instantiate, and remove AimSkillShotTag
            foreach (var skillShot in SystemAPI.Query<SkillShotAspect>().WithAll<AimSkillShotTag, Simulate>())
            {
                if (!skillShot.ConfirmAttack) { continue; }

                var skillShotAbility = ecb.Instantiate(skillShot.AbilityPrefab);
                
                var spawnPosition = skillShot.SpawnPosition;
                ecb.SetComponent(skillShotAbility, spawnPosition);
                ecb.SetComponent(skillShotAbility, skillShot.Team);
                ecb.RemoveComponent<AimSkillShotTag>(skillShot.ChampionEntity);

                if (isServer) { continue; }
                
                skillShot.AbilityCooldownTargetTicks.GetDataAtTick(currentTick, out var currentTargetTicks);

                var newCooldownTargetTick = currentTick;
                newCooldownTargetTick.Add(skillShot.CooldownTicks);
                currentTargetTicks.SkillShotAbilityEndCooldownTick = newCooldownTargetTick;

                var nextTick = currentTick;
                nextTick.Add(1u);
                currentTargetTicks.Tick = nextTick;
                
                skillShot.AbilityCooldownTargetTicks.AddCommandData(currentTargetTicks);
            }
            
            //Cleanup if was casted
            foreach (var (abilityInput, skillShotUIReference, entity) 
                     in SystemAPI.Query<AbilityInput, SkillShotUIReference>().WithAll<Simulate>().WithEntityAccess())
            {
                if (!abilityInput.ConfirmSkillShotAbility.IsSet) continue;
                Object.Destroy(skillShotUIReference.Value);
                ecb.RemoveComponent<SkillShotUIReference>(entity);
            }
            
            //Cleanup if caster died during casting
            foreach (var (skillShotUIReference, entity) 
                     in SystemAPI.Query<SkillShotUIReference>().WithAll<Simulate>().WithNone<LocalTransform>().WithEntityAccess())
            {
                Object.Destroy(skillShotUIReference.Value);
                ecb.RemoveComponent<SkillShotUIReference>(entity);
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}