using Data;
using Unity.Entities;
using UnityEngine;

namespace DOTS
{
    public class BattleGridDataAuthoring : MonoBehaviour
    {
        [SerializeField] 
        private BattleGridData battleGridData;
        
        private class BattleGridDataAuthoringBaker : Baker<BattleGridDataAuthoring>
        {
            public override void Bake(BattleGridDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new BattleGridProperties
                {
                    Width = authoring.battleGridData.width,
                    Height = authoring.battleGridData.height,
                });
            }
        }
    }
}