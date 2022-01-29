using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Umbr.EF.Units
{
    [RequireComponent(typeof(Rigidbody))]
    public class UnitLogic : MonoBehaviour
    {
        private const float VISUAL_DAMAGE_TIME = 0.1f;
        protected float multiplier;

        public UnitStats.Base stats { get; private set; }
        protected UnitType unitType;
        protected List<GameObject> deathReamains;
        public UnitType GetUnitType => unitType;
        protected Rigidbody rg;
        public Vector3 GetMovementDirection => rg.velocity.normalized;

        protected Animator animCtrl;

        private Dictionary<string, float> modifiers;
        protected float health;

        [SerializeField] private Renderer render;
        private Color originColor;

        [SerializeField] private ParticleSystem injuredParticle;
        [SerializeField] private VisualEffect injuredVFX;
        [SerializeField] private ParticleSystem deathParticle;
        public ParticleSystem DeathParticle => deathParticle;

        protected Coroutine coroutine;


        private void Start()
        {
            Umbr.EF.Manager.GameManager.Instance.playerLogic.OnPLayerDeath += PlayerLogic_OnPLayerDeath;
        }


        public void Init(UnitInfo _info, float _multiplier = 1)
        {
            stats = _info.stats;
            unitType = _info.unitType;
            deathReamains = _info.deathRemains;
            multiplier = _multiplier;

            animCtrl = gameObject.GetComponentInChildren<Animator>();

            rg = gameObject.GetComponent<Rigidbody>();
            health = stats.health * multiplier;
            rg.mass = stats.mass * multiplier;

            originColor = render.material.color;

            if (health <= 0)
                Die();

        }

        public virtual void Move(Vector3 position)
        {
            rg.AddForce(position * stats.speed * multiplier, ForceMode.Force);
            MoveAnimation();
        }

        protected void MoveAnimation()
        {
            float speedMode;
            if (rg.velocity.magnitude < 0.0001)
                speedMode = 0;
            else if (rg.velocity.magnitude < 1.5f)
                speedMode = 0.5f;
            else
                speedMode = 1;
            animCtrl.SetFloat("Movement Speed", speedMode, 0.15f, Time.deltaTime);
        }

        public virtual void Attack(UnitLogic enemy, float value, Vector3 direction)
        {
            AttackAnimation();
            enemy.RecieveDamage(value * multiplier, direction, stats.attackPower);
        }

        public void AttackAnimation()
        {
            animCtrl.SetTrigger("Attack");
        }

        public virtual void RecieveDamage(float value, Vector3 direction, float attackPower = 0)
        {
            if (health > 0)
            {
                if (value - stats.armor * multiplier > 0)
                {
                    health -= (value - stats.armor * multiplier);
                    if (coroutine != null)
                        StopCoroutine(coroutine);
                    coroutine = StartCoroutine(RecieveDamageVisual(direction, attackPower));
                }
                if (health <= 0)
                {
                    //Debug.Log(unitType);
                    Die();
                }
            }
        }

        protected IEnumerator RecieveDamageVisual(Vector3 direction, float attackPower)
        {
            // sync with attack anim
            yield return new WaitForSeconds(0.2f);

            render.material.color = Color.red;
            if (injuredParticle != null)
                injuredParticle.Play();

            if (injuredVFX != null)
                injuredVFX.Play();

            rg.AddForce(Vector3.up * attackPower / 4, ForceMode.Impulse);
            rg.AddForce(direction * attackPower, ForceMode.Impulse);

            yield return new WaitForSeconds(VISUAL_DAMAGE_TIME);
            render.material.color = originColor;

        }

        protected virtual void Die()
        {
            if (deathParticle != null)
            {
                var deathParticleInst = Instantiate(deathParticle, transform.position, Quaternion.identity);
                //changed on end destroy in inspector
                //Destroy(deathPart.gameObject, deathPart.main.duration + deathPart.main.startLifetimeMultiplier);
            }

            if (deathReamains.Count > 0)
            {
                deathReamains.OrderBy(elem => Random.Range(0, 1));
                foreach (var deathPrt in deathReamains.GetRange(0, deathReamains.Count-1))
                {
                    var deathPart = Instantiate(deathReamains[Random.Range(0, deathReamains.Count)],
                        transform.position, Random.rotation);
                    deathPart.gameObject.GetComponent<DeathPart>().Kick(Vector3.up * 0.6f);

                }
            }

            StopAllCoroutines();
            Destroy(gameObject);
        }






        /*  ACTIONS RESPONSES  */

        private void PlayerLogic_OnPLayerDeath()
        {
            animCtrl.SetFloat("Movement Speed", 0f, 0.15f, Time.deltaTime);
            StopAllCoroutines();
        }

    }
}
