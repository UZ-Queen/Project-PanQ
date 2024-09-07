using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PauseMenu : MonoBehaviour
{
    public static bool isGamePaused;
    [SerializeField]
    GameObject pauseUI;





    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(isGamePaused)
                Resume();
            else
                Pause();
        }
    }



    public void Pause(){
        pauseUI.SetActive(true);
        isGamePaused = true;
        Time.timeScale = 0f;

    }

    public void Resume(){
        pauseUI.SetActive(false);
        isGamePaused = false;
        Time.timeScale = 1f;
    }

    public void Quit(){
        Application.Quit();
    }
}
