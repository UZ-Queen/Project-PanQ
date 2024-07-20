using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//총을 착용할 수 있는 객체에게 총을 장착.
public class GunController : MonoBehaviour
{
    Gun currentGun;
    public Gun initialGun;

    public Transform leftHand;
    public Transform rightHand;

    void Start()
    {
        EquipGun(initialGun);
    }
    
    void EquipGun(Gun gun){
        if(currentGun != null){
            Destroy(currentGun.gameObject);
        }
    currentGun = Instantiate(gun, leftHand.position, Quaternion.identity, leftHand );
    }
    public void Reload(){
        if(currentGun == null)
            return;
        currentGun?.Reload();
    }
    public void OnTriggerHold(){
        if(currentGun != null){
            currentGun.OnTriggerHold();
        }
    }
    public void OnTriggerRelease(){
        if(currentGun != null){
            currentGun.OnTriggerRelease();
        }
    }
    public void LookCursor(Vector3 point){
        if(currentGun != null){
            currentGun.LookCursor(point);
        }
    }

    public void OverrideGunDamage(){
        
    }

    public Vector3 GunPosition{
        get{
            return leftHand.position;
        }
    }

}
