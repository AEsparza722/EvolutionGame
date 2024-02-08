using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class MotherController : MonoBehaviour, IDamageable
{

    float IDamageable.health { get => health; set => health = value; }

    public static MotherController instance;
    Vector2 movementDirection;
    bool canChangeDirection = true;
    bool canMove = true;
    public CircleCollider2D circleCollider;
    Rigidbody2D rb;
    MeshRenderer meshRenderer;
    [SerializeField] VirusAreaDetection virusDetectionRadius;
    [SerializeField] float setDetectionRadius;
    Color defaultColor;
    
    public List<GameObject> spawnObjects = new List<GameObject>();
    

    [SerializeField] float speed;
    [SerializeField] float health;
    float timeAlive;

    
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float rotationMultiplier = 500f;
    [SerializeField] TMP_Text daysSurvivedText;

    private void Awake()
    {        
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        circleCollider = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();

        virusDetectionRadius = GetComponentInChildren<VirusAreaDetection>();
        virusDetectionRadius.gameObject.GetComponent<CircleCollider2D>().radius = setDetectionRadius;
            

    }

    private void Start()
    {
        defaultColor = meshRenderer.material.GetColor("_FresnelColor");
    }

    private void Update()
    {
        spawnObjects[0].transform.parent.gameObject.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

        if (canChangeDirection && canMove)
        {
            StartCoroutine(MoveCharacter());
        }

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

        TimeAlive();
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

    public void takeDamage(float damage, float force)
    {
        health -= damage;

        //Debug.Log(health);

        StartCoroutine(ChangeColorDamage());
        StartCoroutine(KnockBack(force));

        if (health <= 0)
        {
            GameManager.instance.GameOver();
            Destroy(gameObject);
        }
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

    void TimeAlive()
    {
        timeAlive += Time.deltaTime;
        if(timeAlive >= 10)
        {
            GameManager.instance.daysSurvived += 1;
            timeAlive = 0;
            daysSurvivedText.text = "Days Survived: "+GameManager.instance.daysSurvived.ToString();
        }
    }
}
