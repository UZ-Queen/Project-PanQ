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

    public enum FireMode { SemiAuto, Burst, FullAuto, Nemesis}

    [Header("기본 설정")]
    [SerializeField] int damage = 10;
    [SerializeField][Range(0.001f, 1f)] float stablity = 0.9f;
    [SerializeField][Range(1f, 2000f)] float msBetweenShot = 100f;

    // [SerializeField] bool infinityAmmo = false; 

    [SerializeField][Range(1, 10000)] int pellets = 1;
    [SerializeField][Range(0, 180)] float spreadAngles = 15f;
    [SerializeField] float projVelocity = 20f;
    [SerializeField] float lifeTime = 2f;

    [Header("재장전 관련")]
    [SerializeField] bool infinityAmmo = false; 
    [SerializeField][Range(0,200)] int magSize = 7;
    [SerializeField] int currentAmmo = 7;
    [SerializeField]float reloadTime = 1f;

    [SerializeField]bool isReloading = false;

    
    [Header("사격 모드")]
    public FireMode fireMode = FireMode.FullAuto;
    [SerializeField] int burstCount = 3;
    [SerializeField] float msBetweenBurstShot = 50f;

    [Header("네놈을 추격해 주마")]
    
    [SerializeField] bool isHoming = false;
    [SerializeField] float persuitAccuracy = 240f;
    [SerializeField] string targetTag;


    // [SerializeField]
    // private bool allowSelfAttack = false;
    
    [Header("반동")]
    [SerializeField] Vector2 recoilCoef = new Vector2(0.1f, 0.3f);
    [SerializeField] Vector2 recoilAngleCoef = new Vector2(3f, 6f);
    [SerializeField] float recoilRecoverySpeed = 0.1f;
    // [SerializeField] float recoilAngleRecoverySpeed = 0.1f;

    [SerializeField] float positionClampValue = 0.5f;
    [SerializeField] float angleClampValue = 20f;
    Vector3 smoothDampingVelocity;
    float angleSmoothDampingVelocity;
    public float recoilAngle = 0f;


    [Header("미리 설정해둘 것")] 
    [SerializeField] private LayerMask attackFilter;
    [SerializeField] Transform muzzle;
    [SerializeField] Transform shellEjectingPoint;
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] Shell shellPrefap;
    [SerializeField] AudioClip shotSound;
    AudioSource audioSauce;
    MuzzleFlash muzzleFlash;
    float randomizeVelocityCoef = 0.3f; // 발사체 속도가 이 수치만큼 무작위로 바뀜. ex) 0.3이면 속도가 0.7~1.3배 곱해짐. 몰루의 안정치 비슷한 느낌?

    float nextFireTime = 0;

    [SerializeField] bool hasReleasedTriggerAfterShot = true;



    void Start()
    {
        audioSauce = GetComponent<AudioSource>();
        muzzleFlash = GetComponent<MuzzleFlash>();

        currentAmmo = magSize;
    }

    void LateUpdate(){
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref smoothDampingVelocity, 1/recoilRecoverySpeed);
        if(!isReloading){
            recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref angleSmoothDampingVelocity, 1/recoilRecoverySpeed);
            transform.localEulerAngles += Vector3.left * recoilAngle; //흠...
        }

        if(currentAmmo == 0)
            Reload();
    }
    public void Reload(){
        if(isReloading)
            return;
        if(currentAmmo == magSize)
            return;
        
        StartCoroutine(ReloadAnimation());
    }

    IEnumerator ReloadAnimation(){
        isReloading = true;
        StopCoroutine(Fire());
        // float originalRecoilRecoverySpeed = recoilRecoverySpeed;
        // recoilRecoverySpeed = 100;


        yield return new WaitForSeconds(0.2f);
        // recoilRecoverySpeed = originalRecoilRecoverySpeed;
        // Vector3 initialRotation = Vector3.zero;
        Vector3 initialRotation = transform.localEulerAngles;
        recoilAngle = 0;


        float percent = 0;
        float reloadSpeed = 1/  reloadTime;
        float reloadingAngle = 25f;
        while(percent <=1){
            percent += reloadSpeed * Time.deltaTime;
            float interpolation = -percent * ( percent - 1) * 4;
            float rotateAngle = Mathf.Lerp(0, reloadingAngle, interpolation );
            transform.localEulerAngles = initialRotation + rotateAngle * Vector3.left;

            yield return null;
        }
        currentAmmo = magSize;

        isReloading = false;
    }
    public void LookCursor(Vector3 point){
        if(isReloading)
            return;
        
        transform.LookAt(point);
    }

    IEnumerator Fire()
    {
        if(isReloading)
            yield break;
        if(currentAmmo < 1)
            yield break;

        if (nextFireTime >= Time.time)
            yield break;

        if ((fireMode == FireMode.SemiAuto || fireMode == FireMode.Burst) && !hasReleasedTriggerAfterShot)
            yield break;

        nextFireTime = Time.time + msBetweenShot / 1000;
        if(fireMode == FireMode.Burst || fireMode == FireMode.Nemesis)
            nextFireTime += msBetweenBurstShot /1000 * burstCount;
        else
            burstCount = 1;
    
        // 발싸
        for (int i = 0; i < burstCount; i++) 
        {
            if(currentAmmo < 1)
                yield break;
            if(!infinityAmmo)
                currentAmmo--;

            if (shotSound != null)
                audioSauce.PlayOneShot(shotSound);

            muzzleFlash.Activate();

            Shell newShell = Instantiate(shellPrefap, shellEjectingPoint.position, shellEjectingPoint.rotation);
            for (int k = 0; k < pellets; k++)
                SetProjectile();
            
            // 반동
            transform.localPosition -= Vector3.forward * Random.Range(recoilCoef.x, recoilCoef.y);
            Vector3 clampPosition = transform.localPosition;
            clampPosition.z = Mathf.Clamp(clampPosition.z, -positionClampValue, 1f); // 이 부분은 알아서 수정..
            transform.localPosition = clampPosition;


            recoilAngle += Random.Range(recoilAngleCoef.x, recoilAngleCoef.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0, angleClampValue); // 이 부분은 알아서 수정..



            yield return new WaitForSeconds(msBetweenBurstShot / 1000);
        }
    }

    Projectile SetProjectile()
    {
        Quaternion randomRotation = muzzle.rotation;
        randomRotation *= Quaternion.Euler(Random.Range(-spreadAngles / 2, spreadAngles / 2), Random.Range(-spreadAngles / 2, spreadAngles / 2), 0);
        Projectile newProjectile = Instantiate(projectilePrefab, muzzle.position, randomRotation);

        newProjectile.SetVelocity(projVelocity * Random.Range(1 - randomizeVelocityCoef, 1 + randomizeVelocityCoef));
        newProjectile.SetLifeTime(lifeTime);
        float stablityCoef = Random.Range(stablity, 1f);
        newProjectile.Damage = (int)(stablityCoef * damage);

        newProjectile.rotateSpeed = persuitAccuracy;
        newProjectile.createdBy = transform.parent.parent.tag; // 총 -> 손 -> 원본. 더 깔끔하게 정하고 싶은데..
        newProjectile.targetTag = targetTag;
        //newProjectile.doSelfAttack = allowSelfAttack;
        newProjectile.layerMask = attackFilter;
        newProjectile.IsHoming = isHoming;

        return newProjectile;
    }

    public void OnTriggerHold()
    {
        StartCoroutine(Fire());
        hasReleasedTriggerAfterShot = false;
    }

    public void OnTriggerRelease()
    {

        hasReleasedTriggerAfterShot = true;
    }
}


/*


    void Fire(){
        if(nextFireTime >= Time.time) 
            return;

        if(fireMode != FireMode.FullAuto && !hasReleasedTriggerAfterShot)
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



*/