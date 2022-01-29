using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Umbr.EF.Units
{
    public class UnitStats : ScriptableObject
    {
        [System.Serializable]
        public class Base
        {
            public float health;
            public float speed;
            public float mass;
            public float armor;
            public float attack;
            public float attackPower;
            public float attackSpeed;
        }
    }
}