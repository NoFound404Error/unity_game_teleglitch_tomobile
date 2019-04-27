using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour {
    
    Animator doorAnim;
    public bool stageDoor;
    public bool isMapCleared;
    public bool clearStage;

	void Start () {
        doorAnim = gameObject.GetComponent<Animator>();
        isMapCleared = false;        
	}
	
	void Update () {
		
	}

    private void OnTriggerEnter(Collider coll)
    {
        if (!stageDoor)
        {
            if (coll.gameObject.tag == "Player" || coll.gameObject.tag == "Enemy")
            {
                doorAnim.SetBool("DoorOpen", true);
            }
        }
        else if (stageDoor) { 
            if (coll.gameObject.tag == "Player" && isMapCleared == true)
            {
                doorAnim.SetBool("DoorOpen", true);
                StartCoroutine(NextStage());
            }
        }
    }
    private void OnTriggerExit(Collider coll)
    {
        if (!stageDoor)
        {
            if (coll.gameObject.tag == "Player" || coll.gameObject.tag == "Enemy")
            {
                doorAnim.SetBool("DoorOpen", false);
            }
        }
    }

    void OnTriggerStay(Collider coll)
    {
        if (!stageDoor)
        {
            if (coll.gameObject.tag == "Player" || coll.gameObject.tag == "Enemy")
                doorAnim.SetBool("DoorOpen", true);
        }
    }

    IEnumerator NextStage()
    {
        doorAnim.SetBool("DoorOpen", true);
        yield return new WaitForSeconds(1f);
        clearStage = true;
    }
}
