using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int TotalCoin;
    public int Score = 0;
    private int CoinCollected = 0;

    [Header("UI")]
    public TextMeshProUGUI scoreText; 
    public TextMeshProUGUI winText;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        TotalCoin = GameObject.FindGameObjectsWithTag("Coin").Length;
        ScoreUI();
    }

    

    public void CollectCoin()
    {
        CoinCollected++;
        Score++;

        ScoreUI();

        if (CoinCollected == TotalCoin)
            ShowWinScreen();
    }

    public void ScoreUI()
    {

        Debug.Log("Score: " + Score);

        if (scoreText != null)
        {
            scoreText.text = "Score: " + Score;
        }

    }

    void ShowWinScreen()
    {
        if (winText != null)
        {
            Time.timeScale = 0f;
            winText.text = "You Win!";
            Debug.Log("You Win!");
        }
    }

    void OnEnable()
    {
        Enemy.OnEnemyDied += AddScoreifEnemyDied;
    }
 
    void OnDisable()
    {
        Enemy.OnEnemyDied -= AddScoreifEnemyDied;
    }
 
    void AddScoreifEnemyDied(Enemy EnemyIsDying)
    {
        Score = Score + 10;
        ScoreUI();
        Debug.Log("Score: " + Score);
    }

}   