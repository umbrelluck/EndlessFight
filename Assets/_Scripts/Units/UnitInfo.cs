using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Umbr.EF.Units
{
    [CreateAssetMenu(fileName = "New Unit", menuName = "Unit")]
    public class UnitInfo : ScriptableObject
    {
        //public float health;
        //public float speed;
        //public float mass;
        //public float armor;
        //public float attack;

        public UnitType unitType;
        public GameObject prefab;
        public List<GameObject> deathRemains;

        public UnitStats.Base stats;

    }

    public enum UnitType { player, basic, speedy, tank, boss }
}
