// using Unity.Entities;
//
// namespace DOTS
// {
//     public partial class AbilityInputSystem : SystemBase
//     {
//         private AutoBattlerInputActions _inputActions;
//
//         protected override void OnCreate()
//         {
//             _inputActions = new AutoBattlerInputActions();
//         }
//
//         protected override void OnStartRunning()
//         {
//             _inputActions.Enable();
//         }
//
//         protected override void OnStopRunning()
//         {
//             _inputActions.Disable();
//         }
//
//         protected override void OnUpdate()
//         {
//             var squadSpawnInput = new SquadSpawnInput();
//
//             if (_inputActions.GameplayMap.Q.WasPressedThisFrame())
//             {
//                 squadSpawnInput.SpawnWarriorSquad = true;
//             }
//
//             foreach (var abilityInput in SystemAPI.Query<RefRW<SquadSpawnInput>>())
//             {
//                 abilityInput.ValueRW = squadSpawnInput;
//             }
//         }
//     }
// }