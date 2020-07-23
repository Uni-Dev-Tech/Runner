using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManagerGameplay : MonoBehaviour
{
    public GameObject pause;
    public GameObject settings;
    public GameObject pauseMenu;
    public GameObject deathMenu;
    public Text coinScore, coinScoreBlack;
    public bool restart = false;

    public float musicVolume; // поле хранящие настройки звук
    public float soundEffectsVolume; // поле хранящие настроки звуковых эффектов
    public Slider musicVolumeSlider; // доступ к полям слайдера звука
    public Slider soundEffectsSlider; // доступ к полям слайдера звуковых эффектов

    public static UIManagerGameplay instance; // экземпляр объекта
    public void Awake()
    {
        if (UIManagerGameplay.instance != null)
        {
            Destroy(gameObject);
            return;
        }
        UIManagerGameplay.instance = this;
        DontDestroyOnLoad(this.gameObject); // сохраняем для передачи полей musicVolume и soundEffectsVolume
    }
    public void Start()
    {
        Switch(settings, pause);
        Switch(deathMenu, pause);
        pauseMenu.SetActive(false);

        if(UIkeeper.musicVolume != 0)
        {
            musicVolume = UIkeeper.musicVolume;
        }
        if (UIkeeper.soundEffectsVolume != 0)
        {
            soundEffectsVolume = UIkeeper.soundEffectsVolume;
        }
        UIkeeper.accept = true;
    }
    private void Update()
    {
        // ставим Handle в положение которое было в UIManager
        if (musicVolume != 0)
            musicVolumeSlider.value = musicVolume;
        if (soundEffectsVolume != 0)
            soundEffectsSlider.value = soundEffectsVolume;

        if(Input.GetKeyDown(KeyCode.Escape) && !PlayerControllerv2.death)
        {
            if(pauseMenu.activeSelf == false)
            {
                Time.timeScale = 0f;
                pauseMenu.SetActive(true);
                Switch(settings, pause);
                Switch(deathMenu, pause);
            }
            else
            {
                Switch(settings, pause);
                pauseMenu.SetActive(false);
                Time.timeScale = 1f;
            }
        }
        // Выводим собранное количество монеток на экран
        coinScore.text = PlayerControllerv2.coinScore.ToString();
        coinScoreBlack.text = coinScore.text;
        //При переходе на сцену с главным меню передаем значения настроек звука и удаляем этот объект со скриптом
        if (SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 0 && restart)
        {
            if(!restart)
            {
                UIManager.instance.musicVolume = this.musicVolume;
                UIManager.instance.soundEffectsVolume = this.soundEffectsVolume;
                Destroy(this.gameObject);
            }
            if(restart)
            {
                UIkeeper.musicVolume = this.musicVolume;
                UIkeeper.soundEffectsVolume = this.soundEffectsVolume;
                Destroy(this.gameObject);
            }
        }
    }
    public void ContinuePause()
    {
        Switch(settings, pause);
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }
    public void RestartPause()
    {
        Time.timeScale = 1f;
        restart = true;
        SceneManager.LoadScene(0);
    }
    public void MainMenuPause()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }
    public void SettingsPause()
    {
        Switch(pause, settings);
    }
    public void ExitSettingsPause()
    {
        Switch(settings, pause);
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
    public void Switch(GameObject off, GameObject on)
    {
        off.SetActive(false);
        on.SetActive(true);
    }
}
