using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Umbr.EF.Powerups
{
    [CreateAssetMenu(fileName = "New Booster", menuName = "Powerups/Boosters")]
    public class BoostersInfo : Origin
    {
        [Space(15f)]
        [Header("Stats multipliers")]
        [Space(15f)]

        [SerializeField] private Units.UnitStats.Base multipliers;

        public Units.UnitStats.Base GetMultipliers()
        {
            return multipliers;
        }
    }
}