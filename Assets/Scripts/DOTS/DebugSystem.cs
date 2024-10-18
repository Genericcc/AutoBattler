// using Unity.Burst;
// using Unity.Entities;
// using UnityEngine;
//
// namespace DOTS
// {
//     public partial struct DebugSystem : ISystem
//     {
//         public void OnCreate(ref SystemState state)
//         {
//             state.RequireForUpdate<BattleGridTag>();
//         }
//         
//         public void OnUpdate(ref SystemState state)
//         {
//             state.Enabled = false;
//             
//             var battleGridEntity = SystemAPI.GetSingletonEntity<BattleGridTag>();
//             var grinNodes = SystemAPI.GetBuffer<GridNode>(battleGridEntity);
//
//             foreach (var gridNode in grinNodes)
//             {
//                 Debug.Log($"{gridNode.X}, {gridNode.Y}, i: {gridNode.Index}");
//             }
//         }
//     }
// }