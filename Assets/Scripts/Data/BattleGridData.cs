using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "BattleGridData", menuName = "BattleGrids/BattleGrid Data")]
    public class BattleGridData : ScriptableObject
    {
        public int width;
        public int height;
        
    }
}