using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour
{

    private Coroutine attackCoroutine;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        try
        {
            var attackerLogic = transform.parent.gameObject.GetComponent<Umbr.EF.Units.UnitLogic>();
            var enemyLogic = other.gameObject.GetComponent<Umbr.EF.Units.UnitLogic>();
            if (!attackerLogic.CompareTag(enemyLogic.tag))
                //    attackerLogic.Attack(enemyLogic, attackerLogic.stats.attack,
                //        (enemyLogic.transform.position - attackerLogic.transform.position).normalized);
                attackCoroutine = StartCoroutine(AttackCoroutine(attackerLogic, enemyLogic));
        }
        catch (System.Exception)
        {
            try
            {
                var deathPart = other.gameObject.GetComponent<Umbr.EF.Units.DeathPart>();
                deathPart.Kick(Vector3.up * 0.5f);
                deathPart.Kick((other.transform.position - transform.position) * 0.5f);
            }
            catch (System.Exception)
            {
            }
        }
    }

    IEnumerator AttackCoroutine(Umbr.EF.Units.UnitLogic attackerLogic, Umbr.EF.Units.UnitLogic enemyLogic)
    {
        if (enemyLogic != null)
        {
            attackerLogic.Attack(enemyLogic, attackerLogic.stats.attack,
                (enemyLogic.transform.position - attackerLogic.transform.position).normalized);
            yield return new WaitForSeconds(attackerLogic.stats.attackSpeed);
            attackCoroutine = StartCoroutine(AttackCoroutine(attackerLogic, enemyLogic));
        }
        else if (attackCoroutine!=null) StopCoroutine(attackCoroutine);
    }

    private void OnTriggerExit(Collider other)
    {

        try
        {
            if (attackCoroutine != null)
                StopCoroutine(attackCoroutine);
        }
        catch (System.Exception)
        {
        }
    }
}
