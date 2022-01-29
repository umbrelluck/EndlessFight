using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;

namespace Umbr.EF.UI
{
    public class UIMenus : MonoBehaviour
    {

        public static bool gameIsStopped = false;
        [SerializeField] Transform pauseBtn;
        [SerializeField] Transform pauseMenu;
        [SerializeField] Transform endGameMenu;
        [SerializeField] Transform joysCanvas;
        [SerializeField] TextMeshProUGUI timeElapsed;
        [SerializeField] TextMeshProUGUI enemyCount;

        [SerializeField] AudioMixerSnapshot inNormal;
        [SerializeField] AudioMixerSnapshot inPause;
        [SerializeField] AudioMixerSnapshot inEndGame;
        [SerializeField] Transform endGameMusic;

        private void Start()
        {
            try
            {
                Manager.GameManager.Instance.OnEndGameStatsCalculated += EndGame;
            }
            catch (System.Exception)
            {
                Debug.Log("No player");
            }
        }

        private void EndGame(float time, Dictionary<Units.UnitType, int> enemiesCount)
        {

            //Todo set HighScore

            inEndGame.TransitionTo(0.1f);

            if (endGameMusic != null)
                endGameMusic.GetComponent<AudioSource>().Play();

            pauseBtn.gameObject.SetActive(false);
            joysCanvas.gameObject.SetActive(false);
            endGameMenu.gameObject.SetActive(true);

            timeElapsed.text += (" " +
                Mathf.FloorToInt((time / 60)).ToString() + "m : " + Mathf.FloorToInt((time % 60)).ToString() + "s : " +
                Mathf.FloorToInt(((time % 60) * 1000) % 1000).ToString() + "ms");

            int totalCount = 0;
            foreach (KeyValuePair<Umbr.EF.Units.UnitType, int> kvp in enemiesCount)
                totalCount += kvp.Value;

            enemyCount.text += (" " + totalCount.ToString());
            enemyCount.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text += (" " + enemiesCount[Units.UnitType.basic].ToString());
            enemyCount.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text += (" " + enemiesCount[Units.UnitType.speedy].ToString());
            enemyCount.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text += (" " + enemiesCount[Units.UnitType.tank].ToString());
            enemyCount.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text += (" " + enemiesCount[Units.UnitType.boss].ToString());
        }

        public void PlayGame()
        {
            inNormal.TransitionTo(0.1f);

            Time.timeScale = 1;
            if (pauseBtn)
                pauseBtn.gameObject.SetActive(true);
            Loader.Load(Loader.SceneName.EndlessFight);
            //SceneManager.LoadScene("EndlessFight");
        }

        public void LoadMainMenu()
        {
            inNormal.TransitionTo(0.1f);

            Time.timeScale = 1;
            //SceneManager.LoadScene(0);
            Loader.Load(Loader.SceneName.MainMenuScene);
        }

        public void PauseGame()
        {
            inPause.TransitionTo(0.3f);

            gameIsStopped = true;
            Time.timeScale = 0;

            pauseBtn.gameObject.SetActive(false);
            joysCanvas.gameObject.SetActive(false);
            pauseMenu.gameObject.SetActive(true);
        }

        public void ResumeGame()
        {
            inNormal.TransitionTo(0.5f);

            gameIsStopped = false;
            Time.timeScale = 1;

            pauseBtn.gameObject.SetActive(true);
            joysCanvas.gameObject.SetActive(true);
            pauseMenu.gameObject.SetActive(false);
        }

        public void QuitGame()
        {
            //TODO HiGHSCore
            PlayerPrefs.DeleteAll();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
