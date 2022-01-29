using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Umbr.EF.UI
{
    public class MainMenu : MonoBehaviour
    {

        [SerializeField] private Transform settingsMenu;

        private void Start()
        {
            PlayerPrefs.SetInt("unMute", 1);
            PlayerPrefs.SetFloat("volume", 0);
            settingsMenu.gameObject.SetActive(false);
        }

        public void StartGame()
        {
            Loader.Load(Loader.SceneName.EndlessFight);
        }
        
        public void Settings(bool toSettings)
        {
            settingsMenu.gameObject.SetActive(toSettings);
            transform.GetChild(0).gameObject.SetActive(!toSettings);

        }

        public void QuitGame()
        {
            //TODO highscore
            PlayerPrefs.DeleteAll();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}
