using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {        

    public LayerMask collisionMask;
    public float lifetime = 1f;
    float speed = 10;
    public float damage = 1;
    float skinWidth = 0.1f; // 한 프레임 사이에서 적이 앞으로 움직이는 것과 총알이 다가가는게
                            // 동시에 이뤄질때 충돌이 감지되지 않는것을 방지하기 위한 것
    Transform target;
    float turnSpeed = 20f;
    public bool missile = false;

    private void Start()
    {
        Destroy(gameObject, lifetime);
        target = GameObject.FindGameObjectWithTag("Player").transform;
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);
        if(initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0]);
        }
    }

    void Update()
    {
        float moveDistance = speed * Time.deltaTime;        
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);

        if (target != null)
        {
            if (missile == true)
            {
                Vector3 dir = target.position - transform.position;
                Quaternion lookRotation = Quaternion.LookRotation(dir);
                Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
                transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
            }
        }
    }


    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    

    void CheckCollisions(float moveDistance)    // 적과의 충돌 감지
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // isTrigger가 체크된 물체도 감지
        if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
        }
    }

    void OnHitObject(RaycastHit hit)
    {
        if (gameObject.GetComponent<Explosive>() != null)
        {
            SetSpeed(0);
            if (!gameObject.GetComponent<Explosive>().exploded)
            {
                gameObject.GetComponent<CapsuleCollider>().isTrigger = false;
                gameObject.GetComponent<Explosive>().Explosion(3);
            }
        }
        else
        {
            Destroy(gameObject);
        }
        IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
        if(damageableObject != null && hit.collider.tag != "Obstacles" )
        {
            damageableObject.TakeHit(damage, hit);
            var particleSystem = hit.collider.gameObject.GetComponentInChildren<ParticleSystem>();
            if(particleSystem != null)
                particleSystem.Play();
        }

        
        // GameObject.Destroy(gameObject); // 적을 맞추면 총알은 사라짐
        //gameObject.SetActive(false);
    }

    void OnHitObject(Collider c)
    {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeDamage(damage);
        }

        gameObject.SetActive(false);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if(other.tag == "Player")
    //    {
    //        other.GetComponent<IDamageable>().TakeDamage(damage);
    //        print("적 총알 플레이어 관통!");
    //        gameObject.SetActive(false);
    //    }
    //}
}
