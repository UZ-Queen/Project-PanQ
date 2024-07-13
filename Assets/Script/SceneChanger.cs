using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//[RequireComponent(typeof(Collider))]
public class SceneChanger : MonoBehaviour
{
    
    // [SerializeField]
    // Transform destinationGameObject;
    [SerializeField]
    Vector3 destination;
    Player player;


    [SerializeField]
    private string sceneName;

    void Start(){
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>(); //필요없다면 transform만 가져오자.
    }
    void OnTriggerEnter(Collider other){
        if(other.tag != "Player")
            return;
        ChangeScene();
    }

    void ChangeScene(){
        Debug.Log("빨리 감겠습니다!");
        SceneManager.LoadScene(sceneName);
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        if(player == null){
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            //그럴 일은 없겠지만 혹시 모르니까 ㅇㅇ
            if(player == null)
                return; 
        }
        // if(destinationGameObject == null)
        //     destination = Vector3.zero;
        // else{
        //     destination = destinationGameObject.
        // }
        
        if(destination == null)
            destination = Vector3.zero;

        player.transform.position = destination;

        Destroy(gameObject);
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
