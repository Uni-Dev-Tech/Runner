using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenu; // главное меню (канвас)
    public GameObject settings; // настройки звука и музыка (канвас)

    public float musicVolume; // поле хранящие настройки звук
    public float soundEffectsVolume; // поле хранящие настроки звуковых эффектов
    public Slider musicVolumeSlider; // доступ к полям слайдера звука
    public Slider soundEffectsSlider; // доступ к полям слайдера звуковых эффектов

    public static UIManager instance; // экземпляр объекта
    private void Awake()
    {
        if (UIManager.instance != null)
        {
            Destroy(gameObject);
            return;
        }
        UIManager.instance = this;
        DontDestroyOnLoad(this.gameObject); // сохраняем для передачи полей musicVolume и soundEffectsVolume
    }
    public void StartButtonMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void SettingsMainMenu()
    {
        Switch(mainMenu, settings);
    }
    public void ExitSettings()
    {
        Switch(settings, mainMenu);
    }
    public void ExitMainMenu()
    {
        Application.Quit();
    }
    public void SliderForMusic(float vol)
    {
        musicVolume = vol;
    }
    public void SliderForSoundsEffects(float vol)
    {
        soundEffectsVolume = vol;
    }
    /// <summary>
    /// Метод перехода по настройкам в меню
    /// </summary>
    /// <param name="off">объект отключается</param>
    /// <param name="on">объект включается</param>
    private void Switch(GameObject off, GameObject on)
    {
        off.SetActive(false);
        on.SetActive(true);
    }
    private void Update()
    {
        // ставим Handle в положение которое было в UIManagerGameplay 
        if (musicVolume != 0)
            musicVolumeSlider.value = musicVolume;
        if (soundEffectsVolume != 0)
            soundEffectsSlider.value = soundEffectsVolume;
        //При переходе на сцену с геймплеем передаем значения настроек звука и удаляем этот объект со скриптом
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            UIManagerGameplay.instance.musicVolume = this.musicVolume;
            UIManagerGameplay.instance.soundEffectsVolume = this.soundEffectsVolume;
            Destroy(this.gameObject);
        }
    }
}
