using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Jobs;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] TMP_Text coinsText;
    public float coins;
    [SerializeField] public Vector2 gameArea;
    [SerializeField] GameObject Map;
    public bool isGameOver = false;
    public bool isDraggingVirus = false;
    [SerializeField] TMP_Text daysSurvivedText;
    [SerializeField] TMP_Text maxSurvivedText;
    [SerializeField] GameObject gameOverUI;
    [SerializeField] GameObject pauseUI;
    public int daysSurvived;    
    public int maxDaysSurvived;
    public int maxVirusLevel = 2;

        

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        maxDaysSurvived = PlayerPrefs.GetInt("MaxDaysSurvived");
        
    }

    private void Start()
    {
        generateLimits();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(gameObject.transform.position, gameArea);
    }

    public void UpdateCoins(float newCoins)
    {
        coins += newCoins;
        coinsText.text = coins.ToString();
    }

    void generateLimits()
    {
        GameObject topLimits = new GameObject("TopLimit");
        topLimits.transform.position = new Vector2(0, gameArea.y /2);
        BoxCollider2D colliderTop = topLimits.AddComponent<BoxCollider2D>();
        colliderTop.size = new Vector2(gameArea.x, 1);
        topLimits.transform.SetParent(Map.transform);

        GameObject botLimits = new GameObject("BotLimit");
        botLimits.transform.position = new Vector2(0, - gameArea.y / 2);
        BoxCollider2D colliderBot = botLimits.AddComponent<BoxCollider2D>();
        colliderBot.size = new Vector2(gameArea.x, 1);
        botLimits.transform.SetParent(Map.transform);

        GameObject LeftLimits = new GameObject("LeftLimit");
        LeftLimits.transform.position = new Vector2(-gameArea.x / 2, 0);
        BoxCollider2D colliderLeft = LeftLimits.AddComponent<BoxCollider2D>();
        colliderLeft.size = new Vector2(1, gameArea.y);
        LeftLimits.transform.SetParent(Map.transform);

        GameObject RightLimits = new GameObject("RightLimit");
        RightLimits.transform.position = new Vector2(gameArea.x / 2, 0);
        BoxCollider2D colliderRight = RightLimits.AddComponent<BoxCollider2D>();
        colliderRight.size = new Vector2(1, gameArea.y);
        RightLimits.transform.SetParent(Map.transform);

    }

    public void GameOver()
    {
        if (daysSurvived>maxDaysSurvived || maxDaysSurvived==0)
        {
            PlayerPrefs.SetInt("MaxDaysSurvived", daysSurvived);
            maxDaysSurvived = daysSurvived;
        }
        isGameOver = true;
        gameOverUI.SetActive(true);
        daysSurvivedText.text = "Days Survived: "+daysSurvived.ToString();
        maxSurvivedText.text = "Best: "+maxDaysSurvived.ToString();
                
    }

    public void Restart()
    {
        isGameOver = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    void Pause()
    {
        if (Time.timeScale == 0f)
        {
            pauseUI.SetActive(false);
            PostProcess.instance.PostProcessDefault();
            Time.timeScale = 1f;
        }
        else
        {
            pauseUI.SetActive(true);
            PostProcess.instance.BlackAndWhite();
            Time.timeScale = 0f;
        }
        
    }



}
