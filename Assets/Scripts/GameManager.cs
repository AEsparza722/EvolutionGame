using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Jobs;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] TMP_Text coinsText;
    public float coins;
    [SerializeField] public Vector2 gameArea;
    [SerializeField] GameObject Map;
        

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
        
    }

    private void Start()
    {
        generateLimits();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(gameObject.transform.position, gameArea);
    }

    public void UpdateCoins(int newCoins)
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

}
