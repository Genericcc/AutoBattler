using Data;
using UnityEngine;

namespace Battle
{
    public class BattleGrid : MonoBehaviour
    {
        [SerializeField] 
        private BattleGridData battleGridData;
        
        void OnDrawGizmos() 
        {
            Gizmos.color = Color.green;
            DrawGrid();
        }

        void DrawGrid() 
        {
            for (var x = 0; x < battleGridData.width; x++) 
            {
                for (var y = 0; y < battleGridData.height; y++)
                {
                    var cellCenter = new Vector3(x, 0, y) + new Vector3(1, 0, 1) * (1 / 2f);
                    Gizmos.DrawWireCube(cellCenter, new Vector3(1, 0.1f, 1));
                }
            }
        }
    }
}
