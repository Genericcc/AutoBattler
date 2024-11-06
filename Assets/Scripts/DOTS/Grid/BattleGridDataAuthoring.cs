using Data;
using DOTS.Battle;
using Unity.Entities;
using UnityEngine;

namespace DOTS.Grid
{
    public class BattleGridDataAuthoring : MonoBehaviour
    {
        public BattleGridData battleGridData;
        public TeamType[] playingTeams;

        public float hashCellSize;
        public float hashMultiplier = 19;
        
        private class BattleGridDataAuthoringBaker : Baker<BattleGridDataAuthoring>
        {
            public override void Bake(BattleGridDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new BattleGridDimensions
                {
                    Width = authoring.battleGridData.width,
                    Height = authoring.battleGridData.height,
                });
                AddComponent(entity, new SpatialHashProperties
                {
                    CellSize = authoring.hashCellSize,
                    Multiplier = authoring.hashMultiplier,
                });
                var buffer = AddBuffer<PlayingTeam>(entity);
                foreach (var team in authoring.playingTeams)
                {
                    buffer.Add(new PlayingTeam { Value = team });
                }
            }
        }
    }
}