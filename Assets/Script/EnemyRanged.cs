using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(GunController))]
public class EnemyRanged : Enemy
{
    GunController gunController;
   // [SerializeField]


    protected override void Start()
    {
        base.Start();
        gunController = GetComponent<GunController>();

    }

    protected override void Update()
    {
        if(!hasTarget)
            return;
        Vector3 targetDirection = (target.position - transform.position).normalized;
        Ray ray = new Ray(transform.position, targetDirection );
        RaycastHit hit;
        // 시야에 적이 없다면(장애물에 사선이 막히면) 쏘지 않고 이동.

        if(!Physics.Raycast(ray,out hit, attackRange, LayerMask.GetMask("Player", "Obstacle"), QueryTriggerInteraction.Collide)){
            return;
        }

        if(hit.collider.tag != "Player")
            return;

        Debug.DrawLine(transform.position, targetDirection * 5);
        transform.LookAt(target); // 코루틴 등으로 스무스하게 이동했으면 좋겠는데..
        if(nextAttackTime <= Time.time){
            
            gunController.Fire();
            nextAttackTime = Time.time + secondsBetweenAttack;
        }

            

      //  base.Update();
    }
    // protected override IEnumerator Attack()
    // {
        
    // }
    
    // 근접 적보다 멀리 떨어지면 좋겠는데..
    // protected override void SetDestination()
    // {
    //     base.SetDestination();
    // }

}
