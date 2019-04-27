using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable {

    [Header("Living Entity Properties")]
    public float startingHealth;
    //public GameObject[] lootItems;

    [SerializeField]
    public float health;
    protected bool dead;

    public event System.Action OnDeath;

    protected virtual void Start()
    {
        health = startingHealth;
    }

    public virtual void TakeHit(float damage, RaycastHit hit)
    {
        // 맞은 물체와 어떤 일을 하기 위한 것
        TakeDamage(damage);
        print("맞았다!!");
    }
    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        dead = true;
        if(OnDeath != null)
        {
            OnDeath();
        }

        GameObject.Destroy(gameObject, 2.2f);
    }
}
