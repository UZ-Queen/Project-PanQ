using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LivingEntity : MonoBehaviour, IDamageable
{
    public event Action OnDeath;
    public TextMeshPro damageText;

    [SerializeField]
    protected int health;
    public int initialHealth = 10;
    public  int MaxHealth{get;protected set;}
    protected bool isDead = false;
    public bool IsDead{
        get{
            return isDead;
        }}


    AudioSource audioSauce;
    [SerializeField]
    public AudioClip onHitSound;

    public void TakeHit(int damage, RaycastHit hit = new RaycastHit())
    {
        TakeDamage(damage);
    }

    public void TakeDamage(int damage)
    {



        if(isDead) 
            return;
        if(onHitSound != null)
            //audioSauce.PlayOneShot(onHitSound);
            AudioSource.PlayClipAtPoint(onHitSound, transform.position); //이런건 어떨까?
        if(damage > 0 )
            ShowDamageText(damage);

        health -= damage;
        if(health < 0){
            Die();
        }
    }



    public void Heal(int amount){
        if(isDead)
            return;
        health+=amount;
    }

    [ContextMenu("끄앙!")]
    protected void Die(){
        isDead = true;
        health = 0;


        if(OnDeath != null){
            OnDeath();
        }
        //Destroy(gameObject, 0.5f);
        Destroy(gameObject);
        //Destroy(gameObject, 5f);
        //gameObject.SetActive(false); // 이 방법은 소리 재생 못함
    }

    void ShowDamageText(int damage){
        if(damageText == null)
            return;
        var text = Instantiate(damageText, transform.position, Quaternion.identity, Camera.main.transform);
        Destroy(text, 3f);
        text.text = damage.ToString();
    }



    protected virtual void Start()
    {
        audioSauce = GetComponent<AudioSource>();
        health = initialHealth;
        MaxHealth = initialHealth;
    }

//음....
    protected virtual void OnTriggerEnter(Collider other) {
        ICollectable collecectable = other.GetComponent<ICollectable>();

        if(collecectable != null){
            collecectable.OnPickUp(this);
        }
    }


}
