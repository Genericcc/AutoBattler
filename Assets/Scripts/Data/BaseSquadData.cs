using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public abstract class BaseSquadData : ScriptableObject
    {
        public int squadDataID;
        public int rowUnitCount;
        public int columnUnitCount;
        public Vector2 size;
        public GameObject prefab;
    }
}