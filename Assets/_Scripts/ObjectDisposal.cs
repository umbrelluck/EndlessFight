using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDisposal : MonoBehaviour
{

    private void OnTriggerExit(Collider other)
    {
        //if(Umbr.EF.Manager.SpawnSystem.Instance.spawnedPlatforms[0]==transform.parent)
        if (other.CompareTag("Enemy") || other.CompareTag("PLayer"))
        {
            var unitLogic = other.gameObject.GetComponent<Umbr.EF.Units.UnitLogic>();
            unitLogic.RecieveDamage(float.MaxValue, Vector3.zero);

        }
        else
        {
            try
            {
                var abilityLogic = other.gameObject.GetComponent<Umbr.EF.Powerups.OriginLogic>();
                if (Umbr.EF.Manager.SpawnSystem.Instance.spawnedPowerups.Contains(abilityLogic))
                    Umbr.EF.Manager.SpawnSystem.Instance.spawnedPowerups.Remove(abilityLogic);
                Destroy(abilityLogic.gameObject);
            }
            catch (System.Exception)
            {
                try
                {
                    var deathPart = other.gameObject.GetComponent<Umbr.EF.Units.DeathPart>();
                    Destroy(deathPart.gameObject);
                }
                catch (System.Exception)
                {
                }
            }
        }
    }
}
