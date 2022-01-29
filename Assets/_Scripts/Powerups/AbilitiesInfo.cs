using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Umbr.EF.Powerups
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Powerups/Abilities")]
    public class AbilitiesInfo : Origin
    {
        public AbilityType abilityType;
    }
    public enum AbilityType { ExtraPush, Shield, HomingMisile }
}
