using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Umbr.EF.Units;
using Umbr.EF.Powerups;
using System;
using UnityEngine.Audio;

namespace Umbr.EF.Manager
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance { get { return _instance; } }

        private InputHandler inputHandler;
        private SpawnSystem spawnSystem;

        [SerializeField] private UnitInfo player;
        public Player playerLogic { get; private set; }
        [SerializeField] private List<UnitInfo> enemySettings;
        [SerializeField] private List<AbilitiesInfo> abilitiesSettings;
        [SerializeField] private List<BoostersInfo> boosterSettings;

        [SerializeField] AudioClip deathAudio;

        public List<UnitInfo> GetEnemies => enemySettings;


        public float cameraSpeedRotation;
        public float playerRotation;
        public Transform pivot;
        public Transform pivotX;
        public Transform pivotZ;
        [SerializeField] private Transform platform;
        public float platformSpawnHeight;
        public float platformLife;
        public Transform Platfom => platform;

        //end game section start
        private float timeElapsed = 0f;
        public Dictionary<UnitType, int> defetedEnemiesDict;
        //end game section end

        //sound 
        private float MIN_VOLUME = -80;
        [SerializeField] AudioMixerGroup mixerGroup;
        //end sound

        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(gameObject);
            _instance = this;

            platformSpawnHeight = 5;
            platformLife = 20;

            defetedEnemiesDict = new Dictionary<UnitType, int> {
                { UnitType.basic,0 },
                { UnitType.speedy,0},
                { UnitType.tank,0},
                { UnitType.boss, 0}
            };
        }

        public event Action OnPlayerCreated;

        void Start()
        {
            AudioSource camAudioSource = Camera.main.GetComponent<AudioSource>();
            if (PlayerPrefs.GetInt("unMute") == 1)
            {
                //camAudioSource.mute = true;
                //camAudioSource.volume = PlayerPrefs.GetFloat("volume");
                mixerGroup.audioMixer.SetFloat("masterVolume", PlayerPrefs.GetFloat("volume"));
            }
            else
            {
                //camAudioSource.mute = false;
                mixerGroup.audioMixer.SetFloat("masterVolume", MIN_VOLUME);
            }
            //TODO renove debug
            mixerGroup.audioMixer.SetFloat("masterVolume", 0);

            inputHandler = InputHandler.Instance;
            spawnSystem = SpawnSystem.Instance;

            spawnSystem.SpawnPlatform(platform, Vector3.zero, spawnedFromInspector: true);

            var tmp = Instantiate(player.prefab);
            playerLogic = tmp.gameObject.GetComponent<Player>();
            playerLogic.Init(player);
            playerLogic.OnPLayerDeath += PlayerLogic_OnPLayerDeath;
            OnPlayerCreated?.Invoke(); // start platform coroutine

        }

        void FixedUpdate()
        {
            if (playerLogic != null)
            {
                //inputHandler.RotateCamera();
                inputHandler.HandleMovement();
            }
        }

        void Update()
        {
            if (playerLogic != null)
            {
                //inputHandler.RotateCamera();
                spawnSystem.ManageSpawn(enemySettings, abilitiesSettings, boosterSettings);
            }

            timeElapsed += Time.deltaTime;
            //spawnSystem.SpawnEnemies(null, platform);
        }


        public event Action<float, Dictionary<UnitType, int>> OnEndGameStatsCalculated;
        public void PlayerLogic_OnPLayerDeath()
        {
            playerLogic = null;
            StopAllCoroutines();

            //TODO not delete highscore
            //TODO fix speedy when flying
            //PlayerPrefs.DeleteAll(); //Not on player death but on quit game

            // Delay for player to spawn and show dethParticle
            StartCoroutine(EndGameMenuDelay());
        }

        private IEnumerator EndGameMenuDelay()
        {
            //Camera.main.GetComponent<AudioSource>().PlayOneShot(deathAudio);
            yield return new WaitForSeconds(0.6f);
            OnEndGameStatsCalculated?.Invoke(timeElapsed, defetedEnemiesDict);
        }
    }
}

