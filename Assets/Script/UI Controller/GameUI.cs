using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
// using System.Numerics;

public class GameUI : MonoBehaviour
{

    // [Header("정지 메뉴(옵션)")]
    // [SerializeField] private GameObject optionUI;

    [Header("점수 표시")]
    [SerializeField] private TextMeshProUGUI scoreText;
    public bool isGamePaused = false;


    [Header("게임 오버")]
    public Image fadeImage;
    public GameObject gameOverUI;

    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI yourScoreText;
    [SerializeField] private TextMeshProUGUI restartText;
    [SerializeField] private TextMeshProUGUI bestRecordText;


    [Header("스포너 배너 관련")]
    [SerializeField] RectTransform waveBanner;
    [SerializeField] TextMeshProUGUI waveText;
    [SerializeField] TextMeshProUGUI enemiesCountText;
    [SerializeField] float bannerPopupDuration = 0.5f;
    [SerializeField] float bannerDuration = 1.5f;
    Spawner spawner;

    [Header("플레이어 체력 관련")]
    [SerializeField] private RectTransform hpSprite;

    [Header("스트릭 인디케이터")]
    [SerializeField] private RectTransform streakRemainingTimeSprite;
    public Color coldColor = Color.gray;
    public Color lukeWarmColor = Color.yellow;
    public Color hotColor = Color.red;


    [SerializeField] private Player player;
    Coroutine lastCoroutine;



    //여기서 보관할 게 아닌데..
    bool isGameOver = false;
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<Player>().OnDeath += OnGameOver;
    }
    void Awake()
    {
        spawner = FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;

        player = FindObjectOfType<Player>();


    }

    void Update()
    {
        if (!player.IsDead)
        {
            scoreText.text = Mathf.Clamp(ScoreManager.instance.score, 0, 999999).ToString("D6");
        }



        if (Input.GetKeyDown(KeyCode.R) && isGameOver && !GameManager.instance.isGamePaused)
        {
            StartGame();
        }
        float hp = 0f;
        if (player != null)
        {
            hp = (float)player.Health / player.initialHealth;
        }
        hpSprite.localScale = new Vector3(hp, 1, 1);

        SetStreakGauge();
        

    }

    void SetStreakGauge(){
        Color newColor;
        float interpolation = (float)ScoreManager.instance.streak / ScoreManager.instance.MaxStreak;
        if(ScoreManager.instance.streak < ScoreManager.instance.MaxStreak / 2){
            newColor = Color.Lerp(coldColor, lukeWarmColor, interpolation);
        }
        else{
            newColor = Color.Lerp(lukeWarmColor, hotColor, interpolation);
        }
        streakRemainingTimeSprite.GetComponent<Image>().color = newColor;
        streakRemainingTimeSprite.GetComponent<Image>().material.color = Color.white;
        float remainingTimeRatio = Mathf.Clamp((float)ScoreManager.instance.streakRemainingTime / ScoreManager.instance.streakExpiryTime, 0,1);

        streakRemainingTimeSprite.localScale = new Vector3(remainingTimeRatio,1,1); 
    }


    void OnNewWave(int waveIndex)
    {
        waveText.text = string.Format($"-Wave {Utilities.indexToRomanNumerals[waveIndex + 1]}-");
        string enemiesCount = spawner.waves[waveIndex].isEndless ? "∞" : spawner.waves[waveIndex].enemyCount.ToString();
        enemiesCountText.text = string.Format($"Enemies : {enemiesCount}");
        if (lastCoroutine != null)
        {
            StopCoroutine(lastCoroutine);
        }
        lastCoroutine = StartCoroutine(AnimateBanner());
    }

    IEnumerator AnimateBanner()
    {

        float percent = 0;
        float bannerSpeed = 1 / bannerPopupDuration;
        float direction = 1;
        float originalPosition = -250f;
        // float targetPosition = 0f;


        waveBanner.anchoredPosition = new Vector2(0, originalPosition);
        // float delayToSeeBanner = 2f;
        while (percent >= 0f)
        {
            percent += bannerSpeed * Time.deltaTime * direction;
            if (percent >= 1f && direction == 1)
            {
                percent = 1;
                direction = -1;
                // targetPosition = originalPosition;
                yield return new WaitForSeconds(bannerDuration);
            }
            float smoothingPercent = Mathf.SmoothStep(0, 1, percent);
            waveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-250, 0, smoothingPercent);
            // waveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(waveBanner.anchoredPosition.x, targetPosition, 0.1f);
            yield return null;
        }
    }

    void OnGameOver()
    {
        Cursor.visible = true;
        gameOverUI?.SetActive(true);
        isGameOver = true;
        StartCoroutine(FadeOut(Color.clear, new Color(0, 0, 0, 0.95f), 1f));

        StartCoroutine(PlaceGameOverTexts(0.5f));

    }

    IEnumerator PlaceGameOverTexts(float delayForeachText)
    {
        float animationDuration = 0.2f;
        Vector2 initialSize = new Vector2(2, 2);
        TweenScale(gameOverText.rectTransform, initialSize, Vector2.one, LeanTweenType.easeOutQuint, animationDuration);
        yield return new WaitForSeconds(delayForeachText + animationDuration);

        TweenScale(highScoreText.rectTransform, initialSize, Vector2.one, LeanTweenType.easeOutQuint, animationDuration);
        highScoreText.text = string.Format($"High Score : {ScoreManager.instance.lastHighScore.ToString("D6")}");
        yield return new WaitForSeconds(delayForeachText + animationDuration);

        TweenScale(yourScoreText.rectTransform, initialSize, Vector2.one, LeanTweenType.easeOutQuint, animationDuration);
        yield return new WaitForSeconds(delayForeachText + animationDuration);

        int score = 0;

        while (score < ScoreManager.instance.score)
        {
            score += (int)(Random.Range(0.001f, 0.005f) * ScoreManager.instance.score) + 1;
            score = Mathf.Clamp(score, 0, ScoreManager.instance.score);
            yourScoreText.text = string.Format($"Your Score : {score.ToString("D6")}");
            // yield return new WaitForSeconds(0.1f);
            yield return null;
        }


        if (ScoreManager.instance.achievedHighScore)
        {
            yield return new WaitForSeconds(delayForeachText);
            TweenScale(bestRecordText.rectTransform, initialSize, Vector2.one, LeanTweenType.easeOutQuint, animationDuration);
            yield return new WaitForSeconds(delayForeachText + animationDuration);
        }
        TweenScale(restartText.rectTransform, initialSize, Vector2.one, LeanTweenType.easeOutQuint, animationDuration);


    }

    void TweenScale(RectTransform rect, Vector2 initialSize, Vector2 targetSize, LeanTweenType easeType, float animationDuration)
    {
        rect.gameObject.SetActive(true);
        rect.localScale = initialSize;
        rect.LeanScale(targetSize, animationDuration).setEase(easeType);
    }

    IEnumerator FadeOut(Color from, Color to, float time)
    {
        // if(fadeImage == null)
        //     yield return null;
        float speed = 1 / time;
        float percent = 0;

        while (percent <= 1)
        {
            percent += Time.deltaTime * speed;
            fadeImage.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    IEnumerator FadeInText(TextMeshProUGUI text, float fadeTime)
    {

        yield break;
    }


    public void StartGame()
    {
        SceneManager.LoadScene(SceneName.level);
    }



    // void AnimateText(TextMeshPro, )





}
