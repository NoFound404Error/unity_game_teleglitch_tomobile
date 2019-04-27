using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance;
    
    public Image fadeScreen;
    public Image tutorialScreen;
    public GameObject onGameUI;
    public GameObject gameOverUI;
    public Stage[] stage;
    public Text healthText;

    GameObject player;

    bool gameEnded = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").gameObject;

    }
    

    public void CloseTutorial()
    {
        tutorialScreen.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (gameEnded == false)
        {
            healthText.text = player.GetComponent<PlayerController>().health + " / 200";
            if (player.GetComponent<PlayerController>().health <= 0)
                EndGame();
        }
    }
    

    [ContextMenu("Self Destruct")]
    void EndGame()
    {
        gameEnded = true;
        AudioManager.instance.Play2DSound("LevelComplete");
        fadeScreen.gameObject.SetActive(true);
        StartCoroutine(FadeIn(Color.clear, new Color(0, 0, 0, 0.8f), 1));
        onGameUI.SetActive(false);
        gameOverUI.SetActive(true);
    }

    
    IEnumerator FadeIn(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;
        while (percent < 1f)
        {
            percent += Time.deltaTime * speed;
            fadeScreen.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }
    
    IEnumerator FadeOut(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;
        while (percent < 1f)
        {
            percent += Time.deltaTime * speed;
            fadeScreen.color = Color.Lerp(to, from, percent);
            yield return null;
        }
    }
    
    public void FadeIn()
    {
        fadeScreen.gameObject.SetActive(true);
        StartCoroutine(FadeIn(Color.clear, new Color(0, 0, 0, 1f), 1));
    }
    
    public void FadeOut()
    {
        StartCoroutine(FadeOut(Color.clear, new Color(0, 0, 0, 1f), 1));
        fadeScreen.gameObject.SetActive(false);
    }
    
    public void NextStage(int stageNumber)
    {
        if (stageNumber == 3)
            gameEnded = true;
        FadeIn();
        Destroy(stage[stageNumber - 1].activeStage.gameObject, 1f);
        stage[stageNumber].activeStage.gameObject.SetActive(true);
        player.transform.position = stage[stageNumber].startPosition.position;
        FadeOut();
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene("Main");
    }
}
