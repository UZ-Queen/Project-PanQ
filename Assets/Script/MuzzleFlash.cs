using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    [SerializeField]    GameObject flashHolder;
    [SerializeField]    Sprite[] sprites;
    [SerializeField]    SpriteRenderer[] spriteRenderers;

    [SerializeField]    float flashTime = 0.12f;


    int spriteIndex = 0;
    void Start()
    {
        Deactivate();
    }



    public void Activate(){
        flashHolder.SetActive(true);

        foreach(var spriteRenderer in spriteRenderers){
            spriteIndex = Random.Range(0, sprites.Length);
            spriteRenderer.sprite = sprites[spriteIndex];
        }

        Invoke(nameof(Deactivate), flashTime);
    }

    void Deactivate(){
        flashHolder.SetActive(false);
    }
}
