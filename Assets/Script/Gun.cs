using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(MuzzleFlash))]
public class Gun : MonoBehaviour
{
    [SerializeField]    int damage = 10;
    [SerializeField][Range(0.001f, 1f)]    float stablity = 0.9f;
    [SerializeField][Range(1f, 2000f)]  float msBetweenShot = 100f;
    [SerializeField][Range(1, 10000)]   int pellets = 1;
    [SerializeField][Range(0, 180)] float spreadAngles = 15f;
    [SerializeField]    float projVelocity = 20f;
    [SerializeField]    float lifeTime = 2f;
    [SerializeField]    bool isHoming = false;
    [SerializeField]    float persuitAccuracy = 240f; 
    [SerializeField]    string targetTag;

    // [SerializeField]
    // private bool allowSelfAttack = false;

    [SerializeField]    private LayerMask attackFilter;

    [SerializeField]    Transform muzzle;
    [SerializeField]    Transform shellEjectingPoint;
    [SerializeField]    Projectile projectilePrefab;
    [SerializeField]    Shell shellPrefap;
    [SerializeField]    AudioClip shotSound;
    AudioSource audioSauce;
    MuzzleFlash muzzleFlash;
    float randomizeVelocityCoef = 0.3f; // 발사체 속도가 이 수치만큼 무작위로 바뀜. ex) 0.3이면 속도가 0.7~1.3배 곱해짐. 몰루의 안정치 비슷한 느낌?
    System.Random prng = new System.Random();
    float nextFireTime = 0;

    void Start(){
        audioSauce = GetComponent<AudioSource>();
        muzzleFlash = GetComponent<MuzzleFlash>();
    }

    public void Fire(){
        if(nextFireTime >= Time.time) 
            return;

        nextFireTime = Time.time + msBetweenShot / 1000;
        if(shotSound != null)
            audioSauce.PlayOneShot(shotSound);
        muzzleFlash.Activate();


        Shell newShell = Instantiate(shellPrefap, shellEjectingPoint.position, shellEjectingPoint.rotation);



        for(int i=0;i<pellets;i++){      
            Quaternion randomRotation = muzzle.rotation;
            randomRotation *= Quaternion.Euler(Random.Range(-spreadAngles/2,spreadAngles/2), Random.Range(-spreadAngles/2,spreadAngles/2), 0);
            Projectile newProjectile = Instantiate(projectilePrefab, muzzle.position, randomRotation);

            newProjectile.SetVelocity(projVelocity * Random.Range(1 - randomizeVelocityCoef, 1+randomizeVelocityCoef));
            newProjectile.SetLifeTime(lifeTime);
            float stablityCoef = Random.Range(stablity, 1f);
            newProjectile.Damage = (int)(stablityCoef * damage);

            newProjectile.rotateSpeed = persuitAccuracy;
            newProjectile.createdBy = transform.parent.parent.tag; // 총 -> 손 -> 원본. 더 깔끔하게 정하고 싶은데..
            newProjectile.targetTag = targetTag;
            //newProjectile.doSelfAttack = allowSelfAttack;
            newProjectile.layerMask = attackFilter;
            newProjectile.IsHoming = isHoming;
            }
        
    }


}
