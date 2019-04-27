using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

    GameObject activeRoom;

    private void Start()
    {
        activeRoom = transform.GetChild(0).gameObject;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == ("Player"))
        {
            AudioManager.instance.PlaySound("EnemyLocate", Camera.main.transform.position);
            activeRoom.SetActive(true);
        }
    }
}
