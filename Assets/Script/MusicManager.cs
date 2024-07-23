using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioClip menuMusic;
    [SerializeField] AudioClip mainMusic;
    
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.PlayMusic(menuMusic, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightBracket)){
            AudioManager.instance.PlayMusic(mainMusic, 3f);
        }
    }
}
