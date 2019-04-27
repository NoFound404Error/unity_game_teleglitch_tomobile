using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour, IDamageable {

    public float startingHealth;
    public Material[] blockMats;

    protected float health;
    protected bool dead;
    

    protected virtual void Start()
    {
        health = startingHealth;
    }

    public void TakeHit(float damage, RaycastHit hit)
    {
        // 맞은 물체와 어떤 일을 하기 위한 것
        TakeDamage(damage);
    }
    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health < startingHealth && health >= 4 * startingHealth / 5)
            gameObject.GetComponent<MeshRenderer>().material = blockMats[0];
        else if(health < 4 * startingHealth / 5 && health >= 3 * startingHealth / 5)
            gameObject.GetComponent<MeshRenderer>().material = blockMats[1];
        else if (health < 3 * startingHealth / 5 && health >= 2 * startingHealth / 5)
            gameObject.GetComponent<MeshRenderer>().material = blockMats[2];
        else if (health < 2 * startingHealth / 5 && health >= startingHealth / 5)
            gameObject.GetComponent<MeshRenderer>().material = blockMats[3];
        else if (health <= 0 && !dead)
        {
            Die();
        }
    }

    protected void Die()
    {
        AudioManager.instance.PlaySound("BlockBroken", transform.position); // 벽돌 부서지는 소리
        Destroy(gameObject);
        this.enabled = false;
    }
}
