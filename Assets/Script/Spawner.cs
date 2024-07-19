// using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.Tilemaps;

public class Spawner : MonoBehaviour
{

    [SerializeField]Enemy enemyPrefap;
    [SerializeField]EnemyRanged enemyRangedPrefap;

    public Wave[] waves;
    [System.Serializable]
    public class Wave{
        public bool isEndless;
        public int enemyCount;
        [Range(0,1)]public float chanceToSpawnRanged;

        public float moveSpeed;
        public int damage;
        public int damageRanged;
        public Gun gun;

        public float attackRange;
        public float attackRangeRanged;

        public Color skinColor;
        
        public float msBetweenSpawn;
    }

    MapGenerator mapGenerator;

    int currentWaveIndex = -1;
    Wave currentWave;
    int enemiesRemainingToSpawn = -1;
    int enemiesRangedRemainingToSpawn = -1;
    int enemiesRemainingToNextWave = 1; // 다음 웨이브까지 죽여야 할 적들.
    float nextSpawnTime = -1;


    LivingEntity player;

    [SerializeField]    float campCheckingPeriod = 3f;
    float nextCampCheckingTime;
    [SerializeField]    float campRange = 1.5f; // 검사할 때의 위치에서 이 거리 이상 움직이지 않으면 캠핑으로 간주
    [SerializeField]    bool isCamping = false;

    Vector3 playerOldPosition;
    
    [SerializeField]    bool isDisabled = false; // 플레이어가 죽으면 생성 중단.

    public event System.Action<int> OnNewWave = delegate{};

    void Awake(){
        player = FindObjectOfType<Player>();
        mapGenerator = FindObjectOfType<MapGenerator>();
    }

    void Start()
    {
        
        if(player == null){
            isDisabled = true;
            return;
        }
        player.OnDeath += OnPlayerDeath;

        playerOldPosition = player.transform.position;
        nextCampCheckingTime = Time.time + campCheckingPeriod;

        
        NextWave();
    }


    void Update()
    {
        if(isDisabled)
            return;
        if((enemiesRemainingToSpawn > 0 || currentWave.isEndless) && Time.time > nextSpawnTime){
            StartCoroutine(SpawnEnemy());
            enemiesRemainingToSpawn--;
            nextSpawnTime = Time.time + currentWave.msBetweenSpawn / 1000;
        }

        //Check Camping
        if(nextCampCheckingTime<Time.time){
            if(Vector3.SqrMagnitude(playerOldPosition - player.transform.position) < Mathf.Pow(campRange, 2)){
                isCamping = true;
            }
            else{
                isCamping = false;
            }
            nextCampCheckingTime = Time.time + campCheckingPeriod;
            playerOldPosition = player.transform.position;
        }

    }

    IEnumerator SpawnEnemy(){
        Transform tile;
        if(isCamping){
            tile = mapGenerator.PositionToTile(player.transform.position);
        }
        else
            tile = mapGenerator.GetRandomOpenTile();

        Material tileMat = tile.GetComponent<Renderer>().material;
        Color originalColor = tileMat.color;
        Color flashColor = Color.red;

        float spawnDelay = 1f;
        float animationSpeed = 4f;
        float spawnTime = 0f;


        while(spawnTime < spawnDelay){
            spawnTime += Time.deltaTime;
            tileMat.color = Color.Lerp(originalColor, flashColor, Mathf.PingPong(spawnTime * animationSpeed, 1));
            yield return null;
        }
        tileMat.color = originalColor;

        Enemy randomEnemy;
        bool isRanged = false;
        if(Random.Range(0f, 1f) > currentWave.chanceToSpawnRanged){
            randomEnemy = enemyPrefap;
        }
        else{
            randomEnemy = enemyRangedPrefap;
            isRanged = true;
        }

        Vector3 spawnPoint = tile.position + Vector3.up * 0.2f;
        Enemy newEnemy = Instantiate(randomEnemy, spawnPoint, Quaternion.identity);

        newEnemy.SetChara(currentWave.moveSpeed, currentWave.damage, currentWave.skinColor);


        newEnemy.OnDeath += OnEnemyDeath; // 적이 죽을 경우 실행됨.
    }

    void MovePlayer(){
        if(player ==null)
            return;
        player.transform.position= mapGenerator.PositionToTile(Vector3.one * 0.9f).position + Vector3.up;
    }

    void NextWave(){
        if(currentWaveIndex >= waves.Length - 1){
            Debug.LogWarning($"다음 웨이브가 없습니다. 현재 웨이브 : {currentWaveIndex+1}");
            return;
        }
        MovePlayer();

        currentWaveIndex++;
        currentWave = waves[currentWaveIndex];


        enemiesRemainingToSpawn = currentWave.enemyCount;
        enemiesRemainingToNextWave = currentWave.enemyCount;
        nextSpawnTime = Time.time + 3; // 다음 웨이브 시작 시 5초 뒤 스폰 시작. 나중에 보스전의 경우 10초를 두는 둥 변주 가능할 듯? 
        nextCampCheckingTime = Time.time + 5f;
        Debug.Log($"~웨이브 {currentWaveIndex+1}~");
        OnNewWave(currentWaveIndex);
    }

    void OnPlayerDeath(){
        Debug.Log("플레이어가 죽었습니다. 장비를 정지합니다.");
        isDisabled = true;
    }
    void OnEnemyDeath(){
        enemiesRemainingToNextWave--;
        Debug.Log($"적을 해치웠어요! 다음 웨이브까지 남은 적: {enemiesRemainingToNextWave}");
        if(enemiesRemainingToNextWave == 0){
            NextWave();
        }
    }
}
