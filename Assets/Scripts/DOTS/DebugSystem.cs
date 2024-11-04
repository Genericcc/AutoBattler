// using DOTS.Grid;
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
//             state.RequireForUpdate<GridSystemData>();
//         }
//         
//         public void OnUpdate(ref SystemState state)
//         {
//             var gridSystemData = SystemAPI.GetSingleton<GridSystemData>();
//
//             foreach (var gridNode in gridSystemData.TeamBattleGrid.Nodes)
//             {                    
//                 Debug.Log($"{gridNode.X}, {gridNode.Y}, i: {gridNode.Index}");
//             }
//         }
//     }
// }