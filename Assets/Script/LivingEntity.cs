using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// [RequireComponent(typeof(AudioSource))]
public class LivingEntity : MonoBehaviour, IDamageable
{
    public event Action OnDeath;
    public TextMeshPro damageText;

    [SerializeField] protected ParticleSystem deathEffect;
    [SerializeField]
    public int Health{get;protected set;}
    public int initialHealth = 10;
    public  int MaxHealth{get;protected set;}
    protected bool isDead = false;
    public bool IsDead{
        get{
            return isDead;
        }}


    [SerializeField]
    public AudioClip onHitSound;

    public virtual void TakeHit(int damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if(isDead) 
            return;
        if( deathEffect && Health <= damage){
            ParticleSystem newDeathEffect = Instantiate(deathEffect, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection));
            Destroy(newDeathEffect.gameObject, newDeathEffect.main.startLifetime.constant);
            // Destroy(newDeathEffect, newDeathEffect.startLifetime);
        }
        TakeDamage(damage);
    }

    public virtual void TakeDamage(int damage)
    {
        if(isDead) 
            return;
        if(onHitSound != null)
            AudioManager.instance.PlaySFX(onHitSound, transform.position); //이런건 어떨까?
        if(damage > 0 )
            ShowDamageText(damage);

        Health -= damage;
        if(Health < 0){
            Die();
        }
    }


    // protected virtual ParticleSystem CreateDeathEffect(){
    //     ParticleSystem newDeathEffect = Instantiate(deathEffect, hit.point, Quaternion.FromToRotation(Vector3.forward, -hit.normal));
    // }
    public void Heal(int amount){
        if(isDead)
            return;
        Health+=amount;
    }

    [ContextMenu("끄앙!")]
    protected virtual void Die(){
        isDead = true;
        Health = 0;


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
        Health = initialHealth;
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
