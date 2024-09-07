using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medkit : MonoBehaviour, ICollectable
{

    private void Awake() {
        Destroy(gameObject, 7f);
        transform.localScale = Vector3.one * 2;
        transform.LeanScale(Vector3.zero, 8f).setEase(LeanTweenType.easeOutQuint); 
    }
    public void OnPickUp(LivingEntity entity)
    {
        entity.Heal(entity.MaxHealth / 5);
        AudioManager.instance.PlaySFX2D("Medkit Sound");
        Destroy(gameObject);
    }

    public void Expire(int waveIndex){
        Destroy(gameObject, 0.1f);
    }


    private void OnTriggerEnter(Collider other) {
        

        if(other.CompareTag("Player")){
            OnPickUp(other.GetComponent<LivingEntity>());
        }
    }

}
