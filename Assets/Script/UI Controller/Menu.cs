using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Menu : MonoBehaviour
{

    public bool isGamePaused = false;

    [SerializeField] private GameObject mainMenuHolder;
    [SerializeField] private GameObject optionHolder;

    [SerializeField] private Slider[] sliders;
    [SerializeField] private Toggle[] resToggles;

    [SerializeField] private int[] resolutions;
    [SerializeField] private int currentResIndex = 0;
    [SerializeField] private Toggle isFullScreenToggle;
    void Start()
    {
        

        
        sliders[0].value = AudioManager.instance.masterVolume;
        sliders[1].value = AudioManager.instance.musicVolume;
        sliders[2].value = AudioManager.instance.sfxVolume;

        currentResIndex = PlayerPrefs.GetInt("화면 해상도 인덱스", currentResIndex);
        for(int i=0; i<resToggles.Length; i++){
            resToggles[i].isOn = i == currentResIndex;
        }
        bool isFullScreen = PlayerPrefs.GetInt("전체화면", 0) == 0 ? false : true;
        isFullScreenToggle.isOn = isFullScreen;
        //ToggleFullScreen(isFullScreen);

    }
    public void Play()
    {
        SceneManager.LoadScene("Level");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        mainMenuHolder?.SetActive(true);
        optionHolder.SetActive(false);

        GameManager.instance.isGamePaused = false;
        Time.timeScale = 1f;
    }
    public void Option()
    {
        if(GameManager.instance.isGameOver)
            return;

        mainMenuHolder?.SetActive(false);
        optionHolder.SetActive(true);
        GameManager.instance.isGamePaused = true;
        Time.timeScale = 0f;
    }

    void Update(){
        // Debug.Log("현재 타임스케일: " + Time.timeScale);

        if(Input.GetKeyDown(KeyCode.Escape)){
            ToggleOption();
        }
    }

    public void ToggleOption(){
        if(optionHolder.activeSelf){
            MainMenu();
        }
        else{
            Option();
        }
    }


    public void SetMasterVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Master);

    }
    public void SetMusicVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Music);
    }
    public void SetSfxVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Sfx);
    }

    public void SetResolution(int i)
    {
        if (resToggles[i].isOn)
        {
            float ratio = 16 / 9f;
            Screen.SetResolution(resolutions[i], (int)(resolutions[i] / ratio), false);
            currentResIndex = i;
            PlayerPrefs.SetInt("화면 해상도 인덱스", currentResIndex);
            PlayerPrefs.Save();


        }
    }

    public void ToggleFullScreen(bool isFullScreen)
    {

        foreach (Toggle toggle in resToggles)
        {
            toggle.interactable = !isFullScreen;
        }

        if (isFullScreen)
        {
            Resolution maxRes = Screen.resolutions[Screen.resolutions.Length - 1];
            Screen.SetResolution(maxRes.width, maxRes.height, isFullScreen);

        }
        else
        {
            SetResolution(currentResIndex);
        }

        PlayerPrefs.SetInt("전체화면", isFullScreen ? 1 : 0);
        PlayerPrefs.Save();

    }

}
