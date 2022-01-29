using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Umbr.EF.Units
{
    public class Boss : Enemy
    {
        [SerializeField] private float timeBetweenSpawns;
        [SerializeField] private int enemyCount;
        private Coroutine spawnCoroutine;
        Manager.SpawnSystem spawnSystem;
        Manager.GameManager gameManager;

        private List<Enemy> spawnedMobs;
        public List<Enemy> SpawnedMobs => spawnedMobs;


        void Start()
        {
            spawnSystem = Manager.SpawnSystem.Instance;
            gameManager = Manager.GameManager.Instance;

            spawnedMobs = new List<Enemy>();

            timeBetweenSpawns = 10;
            enemyCount = 1;

            InitPlayerLogic();
        }

        // Update is called once per frame
        void Update()
        {
            if (spawnCoroutine == null && playerLogic != null)
                spawnCoroutine = StartCoroutine(BossSpawnMobs());
            else if (spawnCoroutine != null && playerLogic == null)
                StopCoroutine(spawnCoroutine);
        }

        private void FixedUpdate()
        {
            MovementHandle();
        }

        private IEnumerator BossSpawnMobs()
        {
            //spawn
            for (int i = 0; i < enemyCount * (spawnSystem.waveNumber / spawnSystem.WCNT); i++)
            {
                spawnSystem.SpawnEnemy(gameManager.GetEnemies[Random.Range(0, gameManager.GetEnemies.Count - 1)], spawnSystem.spawnedPlatforms[0], out Enemy mob);
                //spawnSystem.SpawnEnemy(gameManager.GetEnemies[2], spawnSystem.spawnedPlatforms[0], out Enemy mob);
                spawnedMobs.Add(mob);
            }
            yield return new WaitForSeconds(timeBetweenSpawns);

            StartCoroutine(BossSpawnMobs());
        }

        public void RemoveUnit(Enemy unit)
        {
            spawnedMobs.Remove(unit);
        }

        protected override void Die()
        {
            if (spawnCoroutine != null)
                StopCoroutine(spawnCoroutine);
            while (spawnedMobs.Count > 0)
            {
                spawnedMobs[0].RecieveDamage(float.MaxValue, Vector3.zero);
            }

            base.Die();
            Debug.Log("end boss death");
        }

        //private void SpawnMobs(List<UnitInfo> enemies, Transform platform)
        //{
        //    int index;
        //    if (enemyCount == 0)
        //    {
        //        // spawn boss here;
        //        Debug.Log("spawn boss");
        //    }
        //    else
        //        for (int i = 0; i < enemyCount; i++)
        //        {
        //            //create randomize ffor enemy index // ?switch waveNumber/WAVE_COUNT
        //            //random on probability 
        //            spawnSystem.SpawnEnemy(enemies[3], platform);
        //            //SpawnEnemy(enemies[Random.Range(0,index+1)], platform);
        //        }
        //}

    }
}
