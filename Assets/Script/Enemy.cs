using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    [SerializeField]
    protected int attackDamage = 2;
    [SerializeField]
    protected float attackRange = 1f;
    [SerializeField]
    protected float attackSpeed = 3f;
    [SerializeField]
    protected float secondsBetweenAttack = 2f;
    [SerializeField]
    protected float pathUpdatePeriod = 0.25f;

    protected NavMeshAgent pathfinder;
    protected Transform target;
    protected LivingEntity targetEntity;
   // enum AttackType{Melee, Ranged}
    public enum State{Idle, Chasing, Attacking};
    protected State state;
    //AttackType attackType;

    protected  float nextAttackTime = -1;
    protected bool hasTarget = false;

    protected bool hasTouchedTarget = false;

    protected float targetColliderRadius = 0;
    protected float thisColliderRadius = 0;

    protected override void Start()
    {
        base.Start();
        pathfinder = GetComponent<NavMeshAgent>();


        
        state = State.Idle;
        targetEntity = GameObject.FindWithTag("Player")?.GetComponent<LivingEntity>();

        if(targetEntity == null)
            return;
        target = targetEntity.transform;
        //target = GameObject.FindWithTag("Player")?.transform;
        hasTarget = true;
        targetEntity.OnDeath += OnTargetDeath;
        state = State.Chasing;

        targetColliderRadius = target.GetComponent<CapsuleCollider>().radius;
        thisColliderRadius = GetComponent<CapsuleCollider>().radius;

        StartCoroutine(FindTarget());

    }

    protected virtual void Update(){
        if(!hasTarget)
            return;
        float sqrDistanceToTarget = (target.position - transform.position).sqrMagnitude;

        if(nextAttackTime<=Time.time && sqrDistanceToTarget <= Mathf.Pow(attackRange,2) ){
            nextAttackTime = Time.time + secondsBetweenAttack;
            StartCoroutine(Attack());
        }
    }

    protected virtual IEnumerator Attack(){
        Vector3 originalPosition = transform.position;
        Vector3 targetPosition = target.position;

        pathfinder.enabled = false;
        state = State.Attacking;
        hasTouchedTarget = Physics.OverlapSphere(transform.position,thisColliderRadius*1.2f, LayerMask.GetMask("Player")) != null;
        bool hasAppliedDamage = false;

        
        float progress = 0; // 0~1. 1이면 애니메이션 종료
        while(progress <= 1){
            //타깃의 콜라이더와 충돌했고, 데미지를 준 적이 없다면 데미지를 가한다.
            if(hasTouchedTarget && !hasAppliedDamage ){ // && progress >= 0.5f
                hasAppliedDamage = true;
                targetEntity?.TakeDamage(attackDamage);
            }
            
            progress += Time.deltaTime * attackSpeed;
            float interpolation = -progress * ( progress - 1) * 4; // x 절편이 0,1이며 극점의 좌표가 (.5, 1)인 이차함수. x = .5를 기준으로 대칭임.
            transform.position = Vector3.Lerp(originalPosition, targetPosition, interpolation); // 보간값이 0이면 원래 위치, 1이면 적의 위치
            
            yield return null; // 한 프레임만 쉴게요
        }
        state = State.Chasing;
        pathfinder.enabled = true;

    }
    protected virtual IEnumerator FindTarget(){

        while( hasTarget ){
            if(state == State.Chasing)
                SetDestination();
            if(state == State.Idle)
                pathfinder.destination = transform.position;

            yield return new WaitForSeconds(pathUpdatePeriod);
        }
    }

    protected virtual void SetDestination(){

        Vector3 targetDirection = (target.position - transform.position).normalized;
        Vector3 destination = target.position - targetDirection * (targetColliderRadius + thisColliderRadius + attackRange*0.25f) ;

        pathfinder.destination = destination;
    }

    protected override void OnTriggerEnter(Collider other) {
        base.OnTriggerEnter(other);
        Debug.Log($"적: {other.name}과 충돌함!");
        hasTouchedTarget = true;
    }

    protected virtual void OnTargetDeath(){
        hasTarget = false;
        state = State.Idle;
    }

}
