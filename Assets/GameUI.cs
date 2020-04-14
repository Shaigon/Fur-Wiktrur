using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject GameWon;
    public GameObject GameLose;
    bool gameEnd = false;

    void OnGameOver(GameObject gameOverUI)
    {
        gameOverUI.SetActive(true);
        gameEnd = true;
        Guard.OnGuardSpotted -= ShowLoseUI;
        Player.OnReachingEnd -= ShowWinUI;
    }

    void ShowLoseUI()
    {
        OnGameOver(GameLose);
    }

    void ShowWinUI()
    {
        OnGameOver(GameWon);
    }


    void Start()
    {
        Guard.OnGuardSpotted += ShowLoseUI;
        Player.OnReachingEnd += ShowWinUI;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameEnd)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(0);
            }
        }
    }
}
