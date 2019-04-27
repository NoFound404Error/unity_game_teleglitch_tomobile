using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

    public GameObject mainMenuHolder;
    public GameObject optionMenuHolder;

    public Slider[] volumeSliders;

    private void Start()
    {
        volumeSliders[0].value = AudioManager.instance.masterVolumePercent;
        volumeSliders[1].value = AudioManager.instance.musicVolumePercent;
        volumeSliders[2].value = AudioManager.instance.soundEffectVolumePercent;
    }

    public void Play()
    {
        AudioManager.instance.Play2DSound("ButtonSelect");
        SceneManager.LoadScene("Main");
    }

    public void BackToMenu()
    {
        AudioManager.instance.Play2DSound("ButtonSelect");
        SceneManager.LoadScene("Start");
        AudioManager.instance.musicSources[1].enabled = false;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OptionsMenu()
    {
        AudioManager.instance.Play2DSound("ButtonSelect");
        mainMenuHolder.SetActive(false);
        optionMenuHolder.SetActive(true);
    }

    public void MainMenu()
    {
        AudioManager.instance.Play2DSound("ButtonSelect");
        optionMenuHolder.SetActive(false);
        mainMenuHolder.SetActive(true);
    }

    public void SetMasterVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Master);
    }
    public void SetMusicVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Music);
    }
    public void SetSoundEffectVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.SoundEffect);
    }
}
