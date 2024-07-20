using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public Image fadeImage;
    public GameObject gameOverUI;

    public RectTransform waveBanner;
    

    //여기서 보관할 게 아닌데..
    bool isGameOver = false;
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<Player>().OnDeath += OnGameOver;
    }

    // Update is called once per frame
    void Update()
    {
        // GameManager 따위의 클래스를 만들고, 거기에서 상태를 관리해야 되나..
        if(Input.GetKeyDown(KeyCode.R) && isGameOver){
            StartGame();
        }

    }

    void OnGameOver(){
        gameOverUI?.SetActive(true);
        isGameOver = true;
        StartCoroutine(FadeOut(Color.clear, Color.black, 1f));
    }


    IEnumerator FadeOut(Color from, Color to, float time){
        // if(fadeImage == null)
        //     yield return null;
        float speed = 1 /time;
        float percent = 0;

        while(percent<=1){
            percent+=Time.deltaTime * speed;
            fadeImage.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }


    public void StartGame(){
        SceneManager.LoadScene("Main");
    }
}
