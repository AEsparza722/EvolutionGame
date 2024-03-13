using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMotherController : MonoBehaviour, IDamageable
{

    float IDamageable.health { get => health; set => health = value; }
    public float colorSaturation { get => ColorSaturation; set => ColorSaturation = value; }

    public static EnemyMotherController instance;
    Vector2 movementDirection;
    bool canChangeDirection = true;
    bool canMove = true;
    bool canSpawn = true;
    public CircleCollider2D circleCollider;
    Rigidbody2D rb;
    MeshRenderer meshRenderer;
    [SerializeField] EnemyMotherDetection virusDetectionRadius;
    [SerializeField] float setDetectionRadius;
    Color defaultColor;
    
    public List<GameObject> spawnObjects = new List<GameObject>();
    [SerializeField] GameObject childEnemy;
    [SerializeField] List<GameObject> childCount = new List<GameObject>();
    int maxChildren;


    [SerializeField] float speed;
    [SerializeField] public float health;
    [SerializeField] public float maxHealth;
    [SerializeField] float spawnCooldown;
    float ColorSaturation;
    public int currentLevel;
            
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float rotationMultiplier = 500f;

    private void Awake()
    {        
    
        circleCollider = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();

        virusDetectionRadius = GetComponentInChildren<EnemyMotherDetection>();
        virusDetectionRadius.gameObject.GetComponent<CircleCollider2D>().radius = setDetectionRadius;
    }

    private void Start()
    {
        defaultColor = meshRenderer.material.GetColor("_FresnelColor");
        health = maxHealth;
    }

    private void Update()
    {
        float posX = Mathf.Clamp(transform.position.x, -GameManager.instance.gameArea.x / 2, GameManager.instance.gameArea.x / 2);
        float posY = Mathf.Clamp(transform.position.y, -GameManager.instance.gameArea.y / 2, GameManager.instance.gameArea.y / 2);
        transform.position = new Vector2(posX, posY);

        maxChildren = 2 + currentLevel;

        if (canChangeDirection && canMove)
        {
            StartCoroutine(MoveCharacter());
        }

        if (virusDetectionRadius.normalVirusDetected.Count > 0)
        {
            canMove = false;
            Escape();
        }

        if (canSpawn)
        {
            if (childCount.Count < maxChildren)
            {
                StartCoroutine(SpawnChildrenEnemy());
            }                            
        }

        for (int i = 0; i < childCount.Count; i++)
        {            
            if (childCount[i] == null)
            {
                childCount.RemoveAt(i);
                i--;
            }
        }

        ColorOverLife();


        //Rotation
        Quaternion rotationDir = Quaternion.Euler(
            movementDirection.x * rotationMultiplier,
            movementDirection.y * rotationMultiplier,
            movementDirection.x * rotationMultiplier
        );

        // Apply the rotation gradually over time
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            rotationDir,
            rotationSpeed * Time.deltaTime
        );
    }

    IEnumerator MoveCharacter()
    {
        canChangeDirection = false;
        movementDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        rb.velocity = Vector2.zero;
        rb.AddForce(movementDirection * speed, ForceMode2D.Impulse);
        yield return new WaitForSeconds(Random.Range(1f, 5f));
        canChangeDirection = true;
    }

    void Escape()
    {
        canChangeDirection = false;
        movementDirection = (transform.position - virusDetectionRadius.normalVirusDetected[0].transform.position).normalized;
        rb.velocity = Vector2.zero;
        rb.AddForce(movementDirection * speed, ForceMode2D.Impulse);
    }

    public void takeDamage(float damage, float force)
    {
        health -= damage;

      

        StartCoroutine(ChangeColorDamage());
        StartCoroutine(KnockBack(0));

        if (health <= 0)
        {            
            EnemySystem.instance.IncreaseKilledMothers();
            Destroy(gameObject);            
        }
    }

    IEnumerator SpawnChildrenEnemy()
    {
        
        canSpawn = false;
        foreach (GameObject obj in spawnObjects)
        {            
            if(obj.transform.position.x >= GameManager.instance.gameArea.x / 2 || obj.transform.position.y >= GameManager.instance.gameArea.y / 2)
            {
                continue;
            }
            if (obj.transform.position.x <= -GameManager.instance.gameArea.x / 2 || obj.transform.position.y <= -GameManager.instance.gameArea.y / 2)
            {
                continue;
            }

            GameObject enemyVirusInstance = Instantiate(childEnemy, obj.transform.position, Quaternion.identity);
            enemyVirusInstance.GetComponent<EnemyVirus>().virusData = VirusManager.instance.virusData[currentLevel - 1];
            enemyVirusInstance.GetComponent<EnemyVirus>().UpdateVirusData();
            childCount.Add(enemyVirusInstance);
            yield return new WaitForSeconds(.5f);
            if (childCount.Count >= maxChildren) break;

        }
        yield return new WaitForSeconds(spawnCooldown);
        canSpawn = true;
    }

    public IEnumerator takeDamageOverTime(float damage, float times, float seconds)
    {
        for (int i = 0; i < times; i++)
        {
            takeDamage(damage, 1f);
            yield return new WaitForSeconds(seconds);
        }

    }

    public IEnumerator ChangeColorDamage()
    {
        meshRenderer.material.SetColor("_FresnelColor", Color.red);
        yield return new WaitForSeconds(.3f);
        meshRenderer.material.SetColor("_FresnelColor", defaultColor);
    }

    public IEnumerator KnockBack(float force)
    {
        rb.AddForce(-movementDirection * (speed * force), ForceMode2D.Impulse);
        yield return new WaitForSeconds(.2f);
    }

    public void ColorOverLife()
    {
        ColorSaturation = (health / maxHealth) * 1;

        float maxClamp = Mathf.Clamp(ColorSaturation, .2f, 1f);
        float colorHue;
        
        float colorS;
        float colorValue;
        Color.RGBToHSV(meshRenderer.material.GetColor("_AlbedoColor"), out colorHue, out colorS, out colorValue);
     
        meshRenderer.material.SetColor("_AlbedoColor", Color.HSVToRGB(colorHue, ColorSaturation, colorValue));
        meshRenderer.material.SetFloat("_Max", maxClamp);
        if (ColorSaturation < 0.5)
        {
            meshRenderer.material.SetInt("_UseFresnel", 0);
        }
        else
        {
            meshRenderer.material.SetInt("_UseFresnel", 1);
        }

    }

    

}
