using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Umbr.EF.Powerups
{
    public class AbilitiesLogic : OriginLogic
    {
        public AbilitiesInfo ability { get; private set; }
        private Units.Player playerLogic;
        private Coroutine abilityCotoutine;


        private void Start()
        {
            Manager.GameManager.Instance.playerLogic.OnPLayerDeath += PlayerLogic_OnPLayerDeath;
        }

        public void Init(Origin _ability)
        {
            try
            {
                ability = (AbilitiesInfo)_ability;
                rotationSpeed = _ability.rotationSpeed;
            }
            catch (System.Exception)
            {
                Debug.LogError("Ability Init incorrect type");
                throw;
            }
        }

        private void Update()
        {
            base.Rotate();
            base.IdleMovement();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("PLayer"))
            {
                GetComponent<BoxCollider>().enabled = false;

                playerLogic = other.gameObject.GetComponent<Units.Player>();
                playerLogic.AquirePowerup(this);
                //if deleted then player can not use ability
                gameObject.transform.SetParent(playerLogic.gameObject.transform);
                gameObject.transform.position = playerLogic.gameObject.transform.position;
                //gameObject.SetActive(false);
                GetComponentInChildren<Renderer>().enabled = !GetComponentInChildren<Renderer>().enabled;

                //if (abilityCotoutine == null)
                //    abilityCotoutine = StartCoroutine(Dispose());

                //Manager.SpawnSystem.Instance.spawnedPowerups.Remove(this);
            }

        }

        private IEnumerator Dispose()
        {
            yield return new WaitForSeconds(ability.lifeTime);
            Destroy(gameObject);
        }

        public void UseAbility()
        {
            if (abilityCotoutine == null)
            {
                ToggleAbilities(true);
                abilityCotoutine = StartCoroutine(Dispose());
            }
        }

        private void OnDestroy()
        {
            // should remove in spawn from spawnedList?

            if (abilityCotoutine != null)
                StopCoroutine(abilityCotoutine);

            if (Manager.SpawnSystem.Instance.spawnedPowerups.Contains(this))
                Manager.SpawnSystem.Instance.spawnedPowerups.Remove(this);

            if (playerLogic != null)
            {
                ToggleAbilities(false);
                playerLogic.DisposeAbility(this);
            }
        }

        void ToggleAbilities(bool enable)
        {
            switch (ability.abilityType)
            {
                case AbilityType.ExtraPush:
                    playerLogic.gameObject.transform.GetChild(1).gameObject.SetActive(enable);
                    //Debug.Log("Extra Push end"); 
                    break; //extra push on colision in player
                case AbilityType.Shield:
                    playerLogic.gameObject.transform.GetChild(2).gameObject.SetActive(enable);
                    //Debug.Log("Shield end"); 
                    break; //extra push on colision in player
                default: break;
            }
        }



        /*  ACTIONS RESPONSES  */

        private void PlayerLogic_OnPLayerDeath()
        {
            if (this != null)
                StopAllCoroutines();
        }
    }
}
