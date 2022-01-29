using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Umbr.EF.Units;
using Umbr.EF.Powerups;
using System;
using Cinemachine;

namespace Umbr.EF.Manager
{
    public class SpawnSystem : MonoBehaviour
    {
        private static SpawnSystem _instance;
        public static SpawnSystem Instance { get { return _instance; } }

        public List<Enemy> spawnedEnemies { get; private set; }
        //public List<Origin> spawnedPowerups { get; private set; }
        public List<OriginLogic> spawnedPowerups { get; private set; }
        public List<Transform> spawnedPlatforms;
        //public List<Transform> spawnedPlatforms { get; private set; }

        private const int WAVE_COUNT = 5;
        public int WCNT => WAVE_COUNT;
        private const float ABIL_SPAWN_RATE = 15f;
        private Coroutine abilSpawnCoroutine;
        private const float MULT_ADD_ENEMY = 0.1f;
        private const float MULT_ADD_MOB = 0.05f;
        public int waveNumber { get; private set; }
        public int enemyCount { get; private set; }

        private readonly object dieLock = new object();


        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(gameObject);
            _instance = this;

            spawnedEnemies = new List<Enemy>();
            spawnedPowerups = new List<OriginLogic>();
            //spawnedPlatforms = new List<Transform>();
            waveNumber = 1;
        }

        private void Start()
        {
            GameManager.Instance.playerLogic.OnPLayerDeath += PlayerLogic_OnPLayerDeath;
        }


        /*  SPAWNING FUNCTIONS  */

        private Vector3 GetSpawnPosition(Transform platform)
        {
            var platformLogic = platform.gameObject.GetComponent<Platform>();
            Vector3 size = platformLogic.size;
            var center = platformLogic.center;
            return new Vector3(UnityEngine.Random.Range(center.x - size.x / 2 + 1, center.x + size.x / 2 - 1), size.y + 0.1f,
                UnityEngine.Random.Range(center.z - size.z / 2 + 1, center.z + size.z / 2 - 1)); ;
        }

        public void ManageSpawn(List<UnitInfo> enemies, List<AbilitiesInfo> abilities, List<BoostersInfo> boosters)
        {
            if (spawnedEnemies.Count == 0)
                SpawnWave(enemies, abilities, boosters, spawnedPlatforms[0]);

        }

        private void SpawnWave(List<UnitInfo> enemies, List<AbilitiesInfo> abilities, List<BoostersInfo> boosters, Transform platform)
        {
            enemyCount = (waveNumber % WAVE_COUNT) * (waveNumber / WAVE_COUNT + 1);
            int index;
            if (enemyCount == 0)
            {
                SpawnEnemy(enemies[3], platform);
            }
            else
                for (int i = 0; i < enemyCount; i++)
                {
                    switch (waveNumber % WAVE_COUNT)
                    {
                        case 0: index = 0; break;
                        case 1: index = 0; break;
                        case 2: index = 1; break;
                        default: index = 2; break;
                    }
                    //SpawnEnemy(enemies[3], platform);
                    // TODO random on probability 
                    SpawnEnemy(enemies[UnityEngine.Random.Range(0, index + 1)], platform);
                }

            //if (abilSpawnCoroutine != null)
            //    StopCoroutine(abilSpawnCoroutine);

            //int tmpIndex = 0;
            //while (spawnedPowerups.Count > tmpIndex)
            //{
            //    var power = spawnedPowerups[tmpIndex];
            //    if (GameManager.Instance.playerLogic.GetAbilityLogic == power)
            //        tmpIndex += 1;
            //    else
            //    {
            //        spawnedPowerups.RemoveAt(tmpIndex);
            //        Destroy(power.gameObject);
            //    }
            //}

            //SpawnAbility(abilities[UnityEngine.Random.Range(0,2)], platform);
            if (abilSpawnCoroutine == null)
                abilSpawnCoroutine = StartCoroutine(SpawnAbility(abilities));
            //abilSpawnCoroutine = StartCoroutine(SpawnAbility(abilities[UnityEngine.Random.Range(0, 2)], platform));
            waveNumber += 1;
        }

        public void SpawnEnemy(UnitInfo enemy, Transform platform)
        {
            Vector3 position = GetSpawnPosition(platform);

            var enemyLogic = enemy.prefab.gameObject.GetComponent<Enemy>();
            var tmp = Instantiate(enemyLogic, position, enemyLogic.transform.rotation);
            tmp.Init(enemy, 1 + MULT_ADD_ENEMY * (waveNumber / WAVE_COUNT));
            tmp.OnEnemyDeath += Enemy_OnEnemyDeath;
            spawnedEnemies.Add(tmp);
        }

        public void SpawnEnemy(UnitInfo enemy, Transform platform, out Enemy tmp)
        {
            Vector3 position = GetSpawnPosition(platform);

            var enemyLogic = enemy.prefab.gameObject.GetComponent<Enemy>();
            tmp = Instantiate(enemyLogic, position, enemyLogic.transform.rotation);
            tmp.Init(enemy, 1 + MULT_ADD_MOB * (waveNumber / WAVE_COUNT));
            tmp.OnEnemyDeath += Enemy_OnEnemyDeath;
        }

        private IEnumerator SpawnAbility(List<AbilitiesInfo> abilities)
        {

            int tmpIndex = 0;
            while (spawnedPowerups.Count > tmpIndex)
            {
                var power = spawnedPowerups[tmpIndex];
                if (GameManager.Instance.playerLogic.GetAbilityLogic == power)
                    tmpIndex += 1;
                else
                {
                    spawnedPowerups.RemoveAt(tmpIndex);
                    Destroy(power.gameObject);
                }
            }

            //TODO Maybe change algo for abilities to pick spawnpoint
            Vector3 position;
            if (spawnedPlatforms.Count > 1)
                position = GetSpawnPosition(spawnedPlatforms[1]);
            else
            {
                position = GetSpawnPosition(spawnedPlatforms[0]);
            }

            AbilitiesInfo ability = abilities[UnityEngine.Random.Range(0, 2)];

            var abilityLogic = ability.prefab.gameObject.GetComponent<AbilitiesLogic>();
            var tmp = Instantiate(abilityLogic, position + new Vector3(0, 0.5f, 0), abilityLogic.transform.rotation);
            tmp.Init(ability);

            spawnedPowerups.Add(tmp);
            yield return new WaitForSeconds(ABIL_SPAWN_RATE);

            abilSpawnCoroutine = StartCoroutine(SpawnAbility(abilities));
        }

        private void SpawnBooster(BoostersInfo booster, Transform platform)
        {
            Vector3 position = GetSpawnPosition(platform);
        }

        public void SpawnPlatform(Transform platform, Vector3 position, int index = -1, bool spawnedFromInspector = false)
        {
            Transform tmpPlatform;
            if (!spawnedFromInspector)
            {
                tmpPlatform = Instantiate(platform, position, Quaternion.identity);
                //tmpPlatform = Instantiate(platform, position, Quaternion.Euler(0,45,0));
                spawnedPlatforms.Add(tmpPlatform);
            }

            tmpPlatform = spawnedPlatforms[spawnedPlatforms.Count - 1];
            tmpPlatform.GetComponent<Platform>().TimeToSpawnNewPlatform += SpawnSystem_TimeToSpawnNewPlatform;
            tmpPlatform.GetComponent<Platform>().MovementComplete += SpawnSystem_MovementComplete;
            tmpPlatform.GetComponent<Platform>().RemovePowerupsOnOldPlatform += SpawnSystem_RemovePowerupsOnOldPlatform;

            Transform pivotB;
            if (index > -1)
            {
                if (index < 2)
                {
                    position.x = spawnedPlatforms[0].position.x + (position.x - spawnedPlatforms[0].position.x) / 2;
                    pivotB = GameManager.Instance.pivotX;
                }
                else
                {
                    position.z = spawnedPlatforms[0].position.z + (position.z - spawnedPlatforms[0].position.z) / 2;
                    pivotB = GameManager.Instance.pivotZ;
                }

                position.y += GameManager.Instance.platformSpawnHeight;

                pivotB.transform.position = position;

                pivotB.gameObject.SetActive(true);
                GameManager.Instance.pivot.gameObject.SetActive(false);
            }
        }






        /*  ACTIONS RESPONSES*/

        private void SpawnSystem_TimeToSpawnNewPlatform()
        {
            var plat = spawnedPlatforms[0];
            Vector3 size = plat.GetComponent<Platform>().size;
            size.y += 6;

            List<Vector3> directions = new List<Vector3>{
                new Vector3(plat.position.x + size.x, plat.position.y, plat.position.z),
                new Vector3(plat.position.x - size.x, plat.position.y, plat.position.z),
                new Vector3(plat.position.x, plat.position.y, plat.position.z + size.z),
                new Vector3(plat.position.x, plat.position.y, plat.position.z - size.z)
            };

            int tmpIndex = UnityEngine.Random.Range(0, directions.Count);
            var position = directions[tmpIndex];

            //new vector for movement up
            SpawnPlatform(spawnedPlatforms[0], position - new Vector3(0, GameManager.Instance.platformSpawnHeight, 0), tmpIndex);
            //SpawnPlatform(GameManager.Instance.Platfom, position - new Vector3(0, GameManager.Instance.platformSpawnHeight, 0), tmpIndex);
        }

        private void SpawnSystem_MovementComplete()
        {
            if (spawnedPlatforms.Count > 1)
            {
                GameManager.Instance.pivot.position = spawnedPlatforms[1].transform.position;
                spawnedPlatforms.RemoveAt(0);
            }
        }

        private void SpawnSystem_RemovePowerupsOnOldPlatform(Vector3 center, Vector3 size)
        {
            size.y += 6;
            Vector3 minBound = center - size / 2;
            Vector3 maxBound = center + size / 2;


            //if (abilSpawnCoroutine != null)
            //    StopCoroutine(abilSpawnCoroutine);

            //remove all powerups on old platform when new platform spawns
            if (GameManager.Instance.playerLogic != null)
            {
                int tmpIndex = 0;
                for (int i = 0; i < spawnedPowerups.Count; i++)
                {
                    var power = spawnedPowerups[tmpIndex];
                    Debug.Log("there are collisions");
                    bool flag = true;
                    if (power.transform.position.x < minBound.x || power.transform.position.y < minBound.y
                        || power.transform.position.z < minBound.z)
                        flag = false;
                    if (power.transform.position.x > maxBound.x || power.transform.position.y > maxBound.y
                        || power.transform.position.z > maxBound.z)
                        flag = false;
                    if (flag && GameManager.Instance.playerLogic.GetAbilityLogic != power)
                    {
                        spawnedPowerups.RemoveAt(tmpIndex);
                        Destroy(power.gameObject);
                        break;
                    }
                    else
                        tmpIndex += 1;

                }
            }
            else while (spawnedPowerups.Count > 0)
                {
                    var power = spawnedPowerups[0];
                    spawnedPowerups.RemoveAt(0);
                    Destroy(power.gameObject);
                }
        }

        private void Enemy_OnEnemyDeath(Enemy enemy)
        {
            if (spawnedEnemies.Contains(enemy))
                spawnedEnemies.Remove(enemy);
            else
            {
                int ind = 0;
                while (ind < spawnedEnemies.Count)
                {
                    try
                    {
                        if (spawnedEnemies[ind].GetComponent<Boss>().SpawnedMobs.Contains(enemy))
                        {
                            spawnedEnemies[ind].GetComponent<Boss>().RemoveUnit(enemy);
                            break;
                        }
                    }
                    catch (Exception)
                    { }
                    ind += 1;
                }
            }

            lock (dieLock)
            {
                if (!GameManager.Instance.defetedEnemiesDict.ContainsKey(enemy.GetUnitType) && GameManager.Instance.playerLogic != null)
                    GameManager.Instance.defetedEnemiesDict.Add(enemy.GetUnitType, 0);
                GameManager.Instance.defetedEnemiesDict[enemy.GetUnitType] += 1;
            }
        }

        private void PlayerLogic_OnPLayerDeath()
        {
            StopAllCoroutines();
        }
    }
}
