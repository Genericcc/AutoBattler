using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "TestAvailableSquadsData", menuName = "Squads/TestAvailableSquadsData")]
    public class TestSquadsRegister : ScriptableObject
    {
        public List<BaseSquadData> availableSquads;
    }
}