using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    public void TakeHit(int damage, RaycastHit hit = new RaycastHit())
    {
        TakeDamage(damage);
    }

    public void TakeDamage(int damage)
    {
        if(isDead) 
            return;
        
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
    private void Die(){
        isDead = true;
        health = 0;


        if(OnDeath != null){
            OnDeath();
        }
        GameObject.Destroy(gameObject);
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
