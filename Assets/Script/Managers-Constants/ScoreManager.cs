using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{


    public int score{get; private set;} = 0;
    public bool achievedHighScore{get;private set;} = false;
    public int lastHighScore{get; private set;}

    public float streakRemainingTime{
        get{ return lastTimeEnemyKilled + streakExpiryTime - Time.time;}
    }
    public static ScoreManager instance { get; private set;}

    // public static 
    int baseScore = 5;
    int comboBaseScore = 2;
    float lastTimeEnemyKilled = float.MaxValue;
    public float streakExpiryTime{get;} = 1f;

    public int streak{get;private set;} = 0;
    [SerializeField]int maxStreak = 8;
    public int MaxStreak{get{return maxStreak;}}
    bool isPlayerDead = false;
    void Awake(){
        if(instance == null){
            instance = this;

        }
        else{
            Destroy(gameObject);
        }



        Enemy.OnDeathStatic += OnEnemyKilled;
        FindObjectOfType<Player>().OnDeath += OnPlayerDeath;
        score = 0;
        lastHighScore = PlayerPrefs.GetInt("HighScore", 1557);
        achievedHighScore = false;

    }

    void Update(){
        if(GameManager.instance.IsDebug){
            if(Input.GetKey(KeyCode.Home)){
                score+= 500;
            }
            if(Input.GetKeyDown(KeyCode.Delete)){
                PlayerPrefs.SetInt("HighScore", 1557);
                PlayerPrefs.Save();
            }
        }
        
    }

    void OnEnemyKilled(){
        Debug.Log("적을 죽였으니 점수를 얻으세요!");
        if(isPlayerDead){
            return;
        }

        if(lastTimeEnemyKilled+streakExpiryTime >= Time.time)
            streak++;
        else
            streak = 0;
        lastTimeEnemyKilled = Time.time;
        streak = Mathf.Clamp(streak, 0, maxStreak); // 알아서 조정..
        score += baseScore +  comboBaseScore * (int)Mathf.Pow(2, streak);
    }


    //만약 죽어서 다시 시작할 경우 등의 경우에는 이벤트가 두 번 구독될 수도 있다. 이를 막기 위해 플레이어가 죽으면 구독 해제를 해주자
    void OnPlayerDeath(){
        Enemy.OnDeathStatic -= OnEnemyKilled;
        isPlayerDead = true;
        CheckHighScore();
        
        }

    void CheckHighScore(){
        //나중에 스트링을 저장하는클래스를 따로 만들자!
        if(PlayerPrefs.GetInt("HighScore", 1557) < score){
            achievedHighScore = true;
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
        }
        else{
            achievedHighScore = false;
        }
    }
}
