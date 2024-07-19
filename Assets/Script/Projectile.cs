using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    //[SerializeField]
    public string createdBy;
    public bool doSelfAttack = false;

    public string targetTag;

    [SerializeField]
    private Transform target;
    [SerializeField]
    public float rotateSpeed = 60f;

    //타깃이 발사체를 향해 빠르게 올 경우 발사체가 타깃 안으로 이동해서 
    // 충돌하지 않는 상황이 발생할 수 있다. 이때 이 값을 올려주면 된다(즉 충돌 판정이 앞쪽으로 너그러워진다)
    [SerializeField] float skinWidth = 0.1f; 

    public LayerMask layerMask;
    private float velocity = 10;
    private int damage = 1;

    private new Renderer renderer;
    //[SerializeField]
    public bool IsHoming{get;set;}
    //public float persuitAccuracy{get;set;}
    public int Damage
    {
        get
        {
            return damage;
        }
        set
        {
            if (value < 0)
                damage = 0;
            else
                damage = value;
        }
    }

    public void SetVelocity(float velocity)
    {
        this.velocity = velocity;
    }

    public void SetLifeTime(float lifeTime)
    {
        Destroy(gameObject, lifeTime);
    }

    // public string targetTag {
    //     get{
    //         switch(createdBy){
    //             case ""
    //         }
    //     }}

    bool hasAppliedDamage = false;

    void Start(){
        renderer = GetComponent<Renderer>();
        // IsHoming = false;

        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.1f, layerMask, QueryTriggerInteraction.Collide);
        if(colliders.Length >= 1){
            OnHitObject(colliders[0], transform.position);
            //Destroy(gameObject);
        }
    }



    // Update is called once per frame
    void Update()
    {
        float moveDistance = Time.deltaTime * velocity;
        if(IsHoming){
            if(target == null)
                SetTarget();
            
            Persuit();
        }
        CheckCollisions(moveDistance + skinWidth);
        transform.Translate(Vector3.forward * moveDistance);
    }
    
    private void SetTarget(){
        target = GameObject.FindGameObjectWithTag(targetTag)?.transform;
        if(target == null){
            Debug.Log($"{tag} 목표를 찾지 못했습니다.");
            IsHoming = false;
            return;
        }
        IDamageable damageable = target.GetComponent<IDamageable>();
        if(damageable != null){
            damageable.OnDeath += SetTarget;
        }

    }
    void Persuit(){

        if(target == null)
            return;


        Vector3 targetDirection = (target.position - transform.position).normalized;
        Vector3 crossProduct =  Vector3.Cross(transform.forward, targetDirection);
        Debug.DrawLine(transform.position, transform.position + crossProduct);
        if(crossProduct.y > 0){
            renderer.material.SetColor("_Color",new Vector4(1,0,0,1));
        }
        else{
            renderer.material.SetColor("_Color",new Vector4(0,0,1,1));
        }

        transform.Rotate(new Vector3(0,crossProduct.y, 0) * Time.deltaTime * rotateSpeed);
    }

    void CheckCollisions(float moveDistance)
    {
        if(hasAppliedDamage)
            return;
        
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance, layerMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
            hasAppliedDamage = true;
        }

    }

    void OnHitObject(RaycastHit hit)
    {
        //Debug.Log($"{this.name} 발사체가 {hit.collider.gameObject.name} 와 충돌!");
        IsHoming = false;
        IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeHit(Damage, hit.point, transform.forward);
        }

        Vector3 originalSize = transform.localScale;
        transform.SetParent(hit.transform);
        transform.localScale = Vector3.Scale(transform.localScale, Utilities.GetReciprocalVector(hit.transform.localScale));
       
        velocity = 0;
    }

    //hitPoint가 필요한가? 흠
    void OnHitObject(Collider other, Vector3 hitPoint){

        IsHoming = false;
        IDamageable damageableObject = other.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            // damageableObject.TakeDamage(Damage);
             damageableObject.TakeHit(Damage, other.transform.position, transform.forward);
        }

        Vector3 originalSize = transform.localScale;
        transform.SetParent(other.transform);
        transform.localScale = Vector3.Scale(transform.localScale, Utilities.GetReciprocalVector(other.transform.localScale));
       
        velocity = 0;
    }
}



/*


    void OnHitObject(RaycastHit hit)
    {
        //Debug.Log($"{this.name} 발사체가 {hit.collider.gameObject.name} 와 충돌!");
        IsHoming = false;
        IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeHit(Damage, hit.point, hit.normal);
        }

        Vector3 originalSize = transform.localScale;
        // Quaternion originalRotation = transform.rotation;
        transform.SetParent(hit.transform);
        // transform.localScale = originalSize;
        //transform.localScale = Vector3.Scale(transform.localScale,hit.transform.localScale);
        
        transform.localScale = Vector3.Scale(transform.localScale, Utilities.GetReciprocalVector(hit.transform.localScale));
       
        velocity = 0;
        //GameObject.Destroy(this);
    }




*/