using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static int score{get; private set;} = 0;

    int baseScore = 5;
    int comboBaseScore = 2;
    float lastTimeEnemyKilled = 0;
    float streakExpiryTime = 1f;

    int streak = 0;
    void Awake(){
        Enemy.OnDeathStatic += OnEnemyKilled;
        FindObjectOfType<Player>().OnDeath += OnPlayerDeath;
    }
    void OnEnemyKilled(){
        Debug.Log("적을 죽였으니 점수를 얻으세요!");
        if(lastTimeEnemyKilled+streakExpiryTime >= Time.time)
            streak++;
        else
            streak = 0;
        lastTimeEnemyKilled = Time.time;
        streak = Mathf.Clamp(streak, 0, 6); // 알아서 조정..
        score += baseScore +  comboBaseScore * (int)Mathf.Pow(2, streak);
    }


    //만약 죽어서 다시 시작할 경우 등의 경우에는 이벤트가 두 번 구독될 수도 있다. 이를 막기 위해 플레이어가 죽으면 구독 해제를 해주자
    void OnPlayerDeath(){
        Enemy.OnDeathStatic -= OnEnemyKilled;
    }
}
