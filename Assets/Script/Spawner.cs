using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    [SerializeField]
    Enemy enemy;
    public Wave[] waves;
    [Serializable]
    public class Wave{
        public int enemyCount;
        public float msBetweenSpawn;
    }


    int currentWaveIndex = -1;
    Wave currentWave;
    int enemiesRemainingToSpawn = -1;
    int enemiesRemainingToNextWave = 1; // 다음 웨이브까지 죽여야 할 적들.
    float nextSpawnTime = -1;

    void NextWave(){
        if(currentWaveIndex >= waves.Length - 1){
            Debug.LogWarning($"다음 웨이브가 없습니다. 현재 웨이브 : {currentWaveIndex+1}");
            return;
        }

        currentWaveIndex++;
        currentWave = waves[currentWaveIndex];

        enemiesRemainingToSpawn = currentWave.enemyCount;
        enemiesRemainingToNextWave = currentWave.enemyCount;
        nextSpawnTime = Time.time + 5; // 다음 웨이브 시작 시 5초 뒤 스폰 시작. 나중에 보스전의 경우 10초를 두는 둥 변주 가능할 듯? 
        Debug.Log($"~웨이브 {currentWaveIndex+1}~");
    }

    void Start()
    {
        NextWave();
    }


    void Update()
    {
        if(enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime){
            Enemy newEnemy = Instantiate(enemy, Vector3.zero, Quaternion.identity);
            newEnemy.OnDeath += OnEnemyDeath; // 적이 죽을 경우 실행됨.
            enemiesRemainingToSpawn--;

            nextSpawnTime = Time.time + currentWave.msBetweenSpawn / 1000;
        }
    }


    void OnEnemyDeath(){
        
        enemiesRemainingToNextWave--;
        Debug.Log($"적을 해치웠어요! 다음 웨이브까지 남은 적: {enemiesRemainingToNextWave}");
        if(enemiesRemainingToNextWave == 0){
            NextWave();
        }
    }
}
