using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Umbr.EF.Units
{
    public class Player : UnitLogic
    {
        protected Powerups.BoostersInfo booster { get; private set; }
        //protected Powerups.AbilitiesInfo ability { get; private set; }

        protected Powerups.AbilitiesLogic abilityLogic;
        public Powerups.AbilitiesLogic GetAbilityLogic => abilityLogic;
        public void DisposeAbility(Powerups.AbilitiesLogic ability)
        {
            if (ability == abilityLogic)
            {
                HideabilityIndicator();
                abilityLogic = null;
            }
        }

        //todo every round restore 10% of applied damage the previos round?

        public void HideabilityIndicator()
        {
            transform.Find("Indicator").gameObject.SetActive(false);
        }

        //private Coroutine boosterCoroutine, abilityCoroutine;

        public void AquirePowerup(Transform logic, Powerups.BoostersInfo _booster = null)
        {
            //if (boosterCoroutine != null)
            //    StopCoroutine(boosterCoroutine);

            booster = _booster;
            //boosterCoroutine = StartCoroutine(BoostDisposal());
        }

        private IEnumerator BoostDisposal()
        {
            yield return new WaitForSeconds(booster.lifeTime);
            booster = null;
        }

        public void AquirePowerup(Powerups.AbilitiesLogic _ability)
        {
            //if (abilityCoroutine != null)
            //    StopCoroutine(abilityCoroutine);

            //abilityLogic = _ability.gameObject.GetComponent<Powerups.AbilitiesLogic>();
            if (abilityLogic != null)
            {
                Destroy(abilityLogic.gameObject);
            }
            abilityLogic = _ability;
            ShowAbilityIndicator();
            //abilityCoroutine = StartCoroutine(AbilityDisposal());
        }

        public void ShowAbilityIndicator()
        {
            if (abilityLogic != null)
            {
                transform.Find("Indicator").gameObject.SetActive(true);
                transform.Find("Indicator").GetComponent<Renderer>().material = 
                    abilityLogic.transform.GetComponentInChildren<Renderer>().material;
            }
        }

        public void UseAbility()
        {
            if (abilityLogic != null)
                abilityLogic.UseAbility();
            HideabilityIndicator();
        }


        public override void Move(Vector3 position)
        {
            float tmpSpeed = stats.speed;
            if (booster != null)
                tmpSpeed *= booster.GetMultipliers().speed;
            rg.AddForce(position * tmpSpeed, ForceMode.Force);

            MoveAnimation();
        }

        public override void Attack(UnitLogic enemy, float value, Vector3 direction)
        {
            float tmpAttackPower = stats.attackPower;
            if (booster != null)
            {
                tmpAttackPower *= booster.GetMultipliers().attackPower;
                value *= booster.GetMultipliers().attack;
            }

            AttackAnimation();
            enemy.RecieveDamage(value, direction, tmpAttackPower);

            //if (boost != null)
            //    enemy.RecieveDamage(value * boost.attack, position, stats.attackPower * boost.attackPower);
            //else
            //    enemy.RecieveDamage(value, position, stats.attackPower);
        }

        public override void RecieveDamage(float value, Vector3 direction, float attackPower = 0)
        {
            if (health > 0)
            {
                float tmpArmor = stats.armor;
                if (booster != null)
                    tmpArmor *= booster.GetMultipliers().armor;
                if (value - tmpArmor > 0)
                    health -= (value - tmpArmor);
                if (health <= 0)
                    Die();
                if (coroutine != null)
                    StopCoroutine(coroutine);
                coroutine = StartCoroutine(RecieveDamageVisual(direction, attackPower));
            }
        }

        public event Action OnPLayerDeath;

        protected override void Die()
        {
            //Manager.GameManager.Instance.RemovePlayer();
            OnPLayerDeath?.Invoke();
            base.Die();
        }

    }
}
