using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance {get; private set;}
    //흑마법! 오늘 제출해야해.. 어쩔수업당
    public bool isGameOver = false;
    public bool isGamePaused = false;
    [SerializeField] private bool isDebug;
    public bool IsDebug{
        get{return isDebug;}
        private set{isDebug = value;}
    }

    void Awake(){
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }

    void Update(){
        Cheat();
    }

    void Cheat(){
        if(!IsDebug)
            return;
        if(Input.GetKeyDown(KeyCode.Backspace)){
            FindObjectOfType<Player>().TakeDamage(int.MaxValue);
        }
        
    }


    public event Action<Transform> OnPlayerSpawn = delegate{};

    bool hasSubscribed = false;
    //아니 이게 아닌데..
    public void PlayerRespawned(Transform playerT){
        if(hasSubscribed){
            return;
        }

        OnPlayerSpawn(playerT);
        FindObjectOfType<Player>().OnDeath += OnPlayerDeath;
        hasSubscribed = true;


    }


    void OnPlayerDeath(){
        FindObjectOfType<Player>().OnDeath -= OnPlayerDeath;
        hasSubscribed = false;
        


    }




}
