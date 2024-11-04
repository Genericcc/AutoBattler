using Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS
{
    public class SquadRegisterAuthoring : MonoBehaviour
    {
        public TestSquadsRegister testSquadsRegister;
        
        private class SquadRegisterAuthoringBaker : Baker<SquadRegisterAuthoring>
        {
            public override void Bake(SquadRegisterAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                var prefabsBuffer = AddBuffer<SquadData>(entity);
                
                foreach (var squadData in authoring.testSquadsRegister.availableSquads)
                {
                    prefabsBuffer.Add(new SquadData
                    {
                        SquadId = squadData.squadDataID,
                        Prefab = GetEntity(squadData.prefab, TransformUsageFlags.Dynamic),
                        Size = math.int2(squadData.size),
                        RowUnitCount = squadData.rowUnitCount,
                        ColumnUnitCount = squadData.columnUnitCount,
                    });
                }
            }
        }
    }
}