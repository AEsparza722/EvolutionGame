using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMotherController : MonoBehaviour, IDamageable
{

    float IDamageable.health { get => health; set => health = value; }

    public static EnemyMotherController instance;
    Vector2 movementDirection;
    bool canChangeDirection = true;
    bool canMove = true;
    public CircleCollider2D circleCollider;
    Rigidbody2D rb;
    MeshRenderer meshRenderer;
    [SerializeField] EnemyMotherDetection virusDetectionRadius;
    [SerializeField] float setDetectionRadius;
    Color defaultColor;
    
    public List<GameObject> spawnObjects = new List<GameObject>();
    

    [SerializeField] float speed;
    [SerializeField] public float health;
    [SerializeField] public float maxHealth;
        
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float rotationMultiplier = 500f;

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
       

        if (canChangeDirection && canMove)
        {
            StartCoroutine(MoveCharacter());
        }

        if (virusDetectionRadius.normalVirusDetected.Count > 0)
        {
            canMove = false;
            Escape();
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

}