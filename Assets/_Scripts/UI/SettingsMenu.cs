using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    private float MIN_VOLUME = -80;
    private float MIN_SLIDER_VOLUME = -40;
    private float MAX_VOLUME = 10;

    [SerializeField] AudioMixerGroup mixerGroup;

    [SerializeField] private Slider slider;
    [SerializeField] private Toggle mute;
    private bool muted;
    // Start is called before the first frame update
    void Start()
    {
        muted = false;
        ChangeVolume();
    }

    // Update is called once per frame
    public void Mute()
    {
        //Camera.main.GetComponent<AudioSource>().mute = !mute.isOn;
        muted = !mute.isOn;
        PlayerPrefs.SetInt("unMute", (mute.isOn) ? 1 : 0);
        ChangeVolume();
    }

    public void ChangeVolume()
    {
        if (!muted)
        {
            //Camera.main.GetComponent<AudioSource>().volume = slider.value;
            PlayerPrefs.SetFloat("volume", Mathf.Lerp(MIN_SLIDER_VOLUME, MAX_VOLUME, slider.value));
            mixerGroup.audioMixer.SetFloat("masterVolume", Mathf.Lerp(MIN_SLIDER_VOLUME, MAX_VOLUME, slider.value));
            //PlayerPrefs.SetFloat("volume", slider.value);
        }
        else
        {
            PlayerPrefs.SetFloat("volume", MIN_VOLUME);
            mixerGroup.audioMixer.SetFloat("masterVolume", MIN_VOLUME);
        }
    }
}
