using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraPushCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Extra collision Trigger bitch");

        try
        {
            var enemyLogic = other.gameObject.GetComponent<Umbr.EF.Units.Enemy>();
            var playerLogic = gameObject.GetComponentInParent<Umbr.EF.Units.Player>();
            playerLogic.Attack(enemyLogic, playerLogic.stats.attack,
                (enemyLogic.transform.position - playerLogic.transform.position).normalized);
        }
        catch (System.Exception)
        {


        }

    }
}
