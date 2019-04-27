using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Throw : Item
{
    [Header("Throw weapon property")]
    public float throwForce = 5;
    public float msBetweenShots = 500;    
    public GameObject throwItem;

    float nextShotTime;
    Transform weaponHold;
    Transform playerBody;
    Vector3 objectPos;
    GameObject tempGameObject;
    

    private void Start()
    {
        playerBody = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).transform;
        weaponHold = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().weaponHold;
    }

    public void Shoot()
    {
        if (Time.time > nextShotTime && currentCount > 0)
        {
            print("투척!!!");
            nextShotTime = Time.time + msBetweenShots / 1000;
            tempGameObject = Instantiate(throwItem, weaponHold.position, throwItem.transform.rotation);
            tempGameObject.GetComponent<Rigidbody>().AddForce(playerBody.forward * throwForce, ForceMode.Impulse);
            tempGameObject.GetComponent<Explosive>().id = ID;
            tempGameObject.GetComponent<Explosive>().thrown = true;
            
            currentCount--;
        }
        else
            return;
    }

    public void RemoteTrigger()
    {
        print("C4 트리거 동작하나?");
        if(tempGameObject != null)
            tempGameObject.GetComponent<Explosive>().trigger = true;
    }
}
