using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossEnemy : EnemyMovement {

    //State currentState;
    //NavMeshAgent pathfinder;

    [Header("Boss Monster")]
    public ParticleSystem laserEffect;
    public Transform firePoint;
    public Transform laserHead;
    
    public bool useLaser;
    public LineRenderer lineRenderer;

    Transform shootPoint;

    protected override void Start()
    {
        base.Start();
        shootPoint = GameObject.FindGameObjectWithTag("Player").transform;
    }


    void ChaseTarget()  // 레이저를 쏠때 머리가 플레이어를 따라가는 것
    {
        Vector3 dir = shootPoint.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(laserHead.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        laserHead.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    void Laser()    // 레이저 LineRenderer를 쏴준다
    {
        if (!lineRenderer.enabled)
        {
            lineRenderer.enabled = true;
            laserEffect.Play();
        }

        // 레이저 시작과 끝 지정
        lineRenderer.SetPosition(0, laserHead.position);
        lineRenderer.SetPosition(1, shootPoint.position);

        // 레이저 이펙트 만들어준다
        Vector3 dir = firePoint.position - shootPoint.position;
        laserEffect.transform.rotation = Quaternion.LookRotation(dir);
        laserEffect.transform.position = shootPoint.position;
    }

    //IEnumerator LongAttack()
    //{
    //    currentState = State.Attacking;
    //    pathfinder.enabled = false;

    //    Vector3 originalPosition = transform.position;
    //    Vector3 dirToTarget = (target.position - transform.position).normalized;
    //    //Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius);

    //    float attackSpeed = 0.1f;
    //    float percent = 0;

    //    bool hasAppliedDamage = false;

    //    while (percent <= 5)
    //    {
    //        if (percent >= .5f && !hasAppliedDamage)
    //        {
    //            hasAppliedDamage = true;
    //            enemyPistol.Shoot();
    //            //targetEntity.TakeDamage(damage);
    //            print("원거리 Enemy 공격했다!");
    //        }

    //        percent += Time.deltaTime * attackSpeed;
    //        //float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
    //        //transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

    //        yield return null;
    //    }

    //    currentState = State.Chasing;
    //    pathfinder.enabled = true;
    //}
}
