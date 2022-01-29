using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Umbr.EF.Units
{
    public class Enemy : UnitLogic
    {
        protected Player playerLogic;


        private void Start()
        {
            InitPlayerLogic();
        }

        protected void InitPlayerLogic()
        {
            playerLogic = Manager.GameManager.Instance.playerLogic;
            playerLogic.OnPLayerDeath += PlayerLogic_OnPLayerDeath;
        }

        private void FixedUpdate()
        {
            MovementHandle();
        }

        protected void MovementHandle()
        {
            if (playerLogic != null)
            {
                LookAt();
                Move((playerLogic.transform.position - transform.position).normalized);
            }

            //if (transform.position.y < DISPOSAL_HEIGHT)
            //    Die();

        }

        protected void LookAt()
        {
            transform.LookAt(playerLogic.gameObject.transform);
        }

        public event Action<Enemy> OnEnemyDeath;
        protected override void Die()
        {
            OnEnemyDeath?.Invoke(this);
            base.Die();
        }






        /*  ACTIONS RESPONSES  */

        private void PlayerLogic_OnPLayerDeath()
        {
            //it seems sometimes when player dies
            //there are residuals of defeated enemies
            //which unity didn`t take care of
            //so this is to check if enemy is alive
            if (animCtrl != null)
            {
                animCtrl.SetFloat("Movement Speed", 0f, 0.15f, Time.deltaTime);
                StopAllCoroutines();
            }
        }
    }
}
