using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class GameOverUI : MonoBehaviour
{
    public Image fadeImage;
    public GameObject gameOverUI;


    [Header("스포너 배너 관련")]
    Spawner spawner;
    [SerializeField] RectTransform waveBanner;
    [SerializeField] TextMeshProUGUI waveText;
    [SerializeField] TextMeshProUGUI enemiesCountText;
    [SerializeField] float bannerPopupDuration = 0.5f;
    [SerializeField] float bannerDuration = 1.5f;
    Coroutine lastCoroutine;

    //여기서 보관할 게 아닌데..
    bool isGameOver = false;
    // Start is called before the first frame update
    void Start(){
        FindObjectOfType<Player>().OnDeath += OnGameOver;
    }
    void Awake()
    {
        spawner = FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;
    }

    void OnNewWave(int waveIndex){
        waveText.text = string.Format($"-Wave {Utilities.indexToRomanNumerals[waveIndex+1]}-");
        string enemiesCount = spawner.waves[waveIndex].isEndless ?"∞" : spawner.waves[waveIndex].enemyCount.ToString();
        enemiesCountText.text = string.Format($"Enemies : {enemiesCount}");
        if ( lastCoroutine != null){
            StopCoroutine(lastCoroutine);
        }
        lastCoroutine = StartCoroutine(AnimateBanner());
    }

    IEnumerator AnimateBanner(){

        float percent = 0;
        float bannerSpeed = 1/  bannerPopupDuration;
        float direction = 1;
        // float delayToSeeBanner = 2f;
        while(percent >= 0f){
            percent += bannerSpeed * Time.deltaTime * direction;
            if(percent >=1f && direction == 1){
                percent = 1;
                direction = -1;
                yield return new WaitForSeconds(bannerDuration);
            }
            waveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-250, 0, percent);
            yield return null;
        }
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
