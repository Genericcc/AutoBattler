using System;
using Unity.Mathematics;
using UnityEngine;

namespace Data
{
    [Serializable]
    public abstract class BaseSquadData : ScriptableObject
    {
        public int squadDataID;
        public Vector2 size;
        
        public GameObject prefab;
    }
}