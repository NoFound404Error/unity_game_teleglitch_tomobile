using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour {

    public int enemyCount;
    public int stageNumber;
    public Transform startPosition;
    public SlidingDoor EndDoor;
    public GameObject activeStage;

    GameManager gameManager;

    private void Start()
    {
        activeStage = transform.GetChild(0).gameObject;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    private void Update()
    {
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (EndDoor != null)
        {
            if (enemyCount == 0)
                EndDoor.isMapCleared = true;
            else
                EndDoor.isMapCleared = false;
        }

        if (EndDoor != null && EndDoor.clearStage == true)
        {
            NextStage(stageNumber + 1);
        }
    }

    void NextStage(int stageNumber)
    {
        gameManager.NextStage(stageNumber);
    }
}
