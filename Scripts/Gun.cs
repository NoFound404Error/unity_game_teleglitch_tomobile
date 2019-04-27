using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : Item {
    

    [Header("Gun property")]
    public Transform[] muzzles;    // 총구
    public Projectile projectile;   // 총알
    public AudioClip fireSound;

    public float msBetweenShots = 100;  // 총알이 발사되는 시간 간격(ms단위)
    public float muzzleVelocity = 35;   // 총알이 발사되는 순간의 총알 속력
    public int weaponNum;               // 총 번호
    public string weaponName;           // 총 이름
    public int burstCount;
    

    private void Start()
    {
        if (gameObject.activeSelf == false)
            gameObject.SetActive(true);
    }

    float nextShotTime;    

    public void Shoot()
    {
        if (Time.time > nextShotTime && currentCount > 0)
        {
            for (int i = 0; i < muzzles.Length; i++)
            {
                nextShotTime = Time.time + msBetweenShots / 1000;
                Projectile newProjectile = Instantiate(projectile, muzzles[i].position, muzzles[i].rotation) as Projectile;
                newProjectile.SetSpeed(muzzleVelocity);
                AudioManager.instance.PlaySound(fireSound, transform.position); 
            }
            currentCount--;
        }
    }
}
