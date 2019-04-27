using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour {

    public float[] explodeRadius;
    public float[] explosionForce;
    public int[] damage;
    public bool thrown ;
    public bool trigger;
    public bool exploded;

    public AudioClip explosionSound;
    
    public int id;  // 0번 : Exp, 7번 : 지뢰, 8번 : C4
    float countdown;

    private void Awake()
    {
        thrown = false;
        trigger = false;
    }
    void Start () {
        if (thrown == true)
            StartCoroutine(WaitExplosion(id));
    }

    IEnumerator WaitExplosion(int id)
    {
        yield return new WaitForSeconds(2f);
        if (id == 0)
            Explosion(0);
        else
            Explosion(2);

    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == ("Enemy") && id == 7)
        {
            Explosion(1);
        }
    }

    public void Explosion(int id)
    {
        AudioManager.instance.PlaySound(explosionSound, transform.position);

        exploded = true;
        
        var particleSystem = GetComponentInChildren<ParticleSystem>();      //이펙트 효과
        if (id != 2)
        {
            particleSystem.Play();
        }
        else
        {
            for (int i = 4; i < gameObject.transform.childCount; i++)
            {
                particleSystem = gameObject.transform.GetChild(i).GetComponent<ParticleSystem>();
                particleSystem.Play();
            }
        }

        switch (id)
        {
            case 0:     // Explosive
                gameObject.GetComponent<MeshRenderer>().enabled = false;
                break;
            case 1:     // Mine
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                break;
            case 2:     // C4
                for (int i = 0; i < 3; i++)
                    gameObject.transform.GetChild(i).GetComponent<MeshRenderer>().enabled = false;
                break;
            case 3:     // Missile
                gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
                break;
        }
       
        Collider[] colliders = Physics.OverlapSphere(transform.position, explodeRadius[id]);
        
        foreach (Collider nearbyObject in colliders)
        {
            // Add Force & Damage
            if (id != 3)
            {
                if (nearbyObject.gameObject.tag == "Enemy")
                {
                    Rigidbody rigidbody = nearbyObject.GetComponent<Rigidbody>();
                    if (rigidbody != null)
                    {
                        rigidbody.AddExplosionForce(explosionForce[id], transform.position, explodeRadius[id]);
                        nearbyObject.GetComponent<LivingEntity>().TakeDamage(damage[id]);                        
                    }
                }    
                else if(nearbyObject.gameObject.tag == "Wall")
                {
                    nearbyObject.GetComponent<Block>().TakeDamage(damage[id]);
                }

            }
            else if(id == 3)
            {
                if (nearbyObject.gameObject.tag == "Player")
                {
                    Rigidbody rigidbody = nearbyObject.GetComponent<Rigidbody>();
                    if (rigidbody != null)
                    {
                        rigidbody.AddExplosionForce(explosionForce[id], transform.position, explodeRadius[id]);
                        nearbyObject.GetComponent<LivingEntity>().TakeDamage(damage[id]);
                    }
                }
                else if (nearbyObject.gameObject.tag == "Wall")
                {
                    nearbyObject.GetComponent<Block>().TakeDamage(damage[id]);
                }
            }
        }
        Destroy(gameObject, 1f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explodeRadius[2]);
    }
}
