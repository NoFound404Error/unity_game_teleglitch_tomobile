using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : LivingEntity {
    
    public enum State { Idle, Chasing, Attacking };
    [Header("Enemy Properties")]    
    public bool longRange;      // 근거리 공격을 하는 적인지 원거리 공격을 하는 적인지 구분
    public ParticleSystem deathEffect;
    public Gun enemyPistol;
    public Transform RotateBody;

    State currentState;
    
    NavMeshAgent pathfinder;
    Transform target;
    LivingEntity targetEntity;
    Animator enemyAnim;
    
    float[] attackDistanceThreshold = { 1.0f, 8.0f };       // 공격 범위
    float timeBetweenAttacks = 1;
    float damage = 1;
    float[] nextAttackTime = { 1f, 10f };
    float myCollisionRadius;
    float targetCollisionRadius;
    public float turnSpeed = 20f;

    bool hasTarget;

    void Awake()
    {
        enemyAnim = transform.GetChild(0).GetComponent<Animator>();
        pathfinder = GetComponent<NavMeshAgent>();
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            hasTarget = true;
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();
            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
        }
    }
    
    protected override void Start()
    {
        base.Start();
        
        if (hasTarget)
        {
            currentState = State.Chasing;
            targetEntity.OnDeath += OnTargetDeath;
            StartCoroutine(UpdatePath());
        }
    }

    public virtual void Update()
    {
        if (hasTarget)
        {
            Vector3 dir = target.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 rotation = Quaternion.Lerp(RotateBody.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
            RotateBody.rotation = Quaternion.Euler(0f, rotation.y, 0f);

            if (!longRange)
            {
                if (Time.time > nextAttackTime[0])
                {
                    float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;

                    if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold[0] + myCollisionRadius + targetCollisionRadius, 2))
                    {
                        nextAttackTime[0] = Time.time + timeBetweenAttacks;

                        StartCoroutine(CloseAttack());
                    }
                }
            }
            else
            {
                if (Time.time > nextAttackTime[1])
                {
                    float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;

                    if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold[1] + myCollisionRadius + targetCollisionRadius, 2))
                    {
                        nextAttackTime[1] = Time.time + timeBetweenAttacks;

                        StartCoroutine(LongAttack());
                    }
                }
            }
        }
        
    }

    public override void TakeHit(float damage, RaycastHit hitPoint)
    {
        if (damage >= health)
        {
            print("Enemy 죽는 애니메이션");
            enemyAnim.SetBool("Death", true);
            GetComponent<CapsuleCollider>().enabled = false;
            hasTarget = false;
            Destroy(Instantiate(deathEffect.gameObject, hitPoint.point, Quaternion.FromToRotation(Vector3.forward, hitPoint.point)) as GameObject, 1f);            
        }

         base.TakeHit(damage, hitPoint);
    }
    
    void OnTargetDeath()
    {
        hasTarget = false;

        currentState = State.Idle;

    }
    
    IEnumerator CloseAttack()
    {
        currentState = State.Attacking;
        pathfinder.enabled = false;
        
        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        //Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius);

        float attackSpeed = 3;
        float percent = 0;
        
        bool hasAppliedDamage = false;
        
        while (percent <= 1)
        {
            if (percent >= .5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                enemyAnim.SetTrigger("Attack");
                targetEntity.TakeDamage(damage);
            }
            
            percent += Time.deltaTime * attackSpeed;
            
            yield return null;
        }
        
        currentState = State.Chasing;
        enemyAnim.SetTrigger("Chasing");
        pathfinder.enabled = true;
    }

    IEnumerator LongAttack()
    {
        currentState = State.Attacking;
        pathfinder.enabled = false;

        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;

        float attackSpeed = 0.5f;
        float percent = 0;

        bool hasAppliedDamage = false;

        while (percent <= 5)
        {
            if (percent >= .5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;

                enemyPistol.Shoot();
            }

            percent += Time.deltaTime * attackSpeed;

            yield return null;
        }

        currentState = State.Chasing;
        enemyAnim.SetTrigger("Chasing");
        pathfinder.enabled = true;
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = 0.5f;
        
        while (hasTarget)
        {
            if (currentState == State.Chasing)
            {
                enemyAnim.SetTrigger("Chasing");
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold[0] /2);
                if (!dead)
                {
                    pathfinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }

    //private void OnTriggerEnter(Collider collider)
    //{
    //    if(collider.gameObject.tag == "Mine")
    //    {
    //        var particleSystem = collider.GetComponentInChildren<ParticleSystem>();
    //        TakeDamage(3f);
    //        particleSystem.Play();
    //        Destroy(collider.gameObject, 0.5f);
    //    }
    //}
}
