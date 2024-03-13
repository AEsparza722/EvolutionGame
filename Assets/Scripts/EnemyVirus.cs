using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;

public class EnemyVirus : MonoBehaviour, IDamageable
{
    float IDamageable.health { get => health; set => health = value; }
    public float colorSaturation { get => ColorSaturation; set => ColorSaturation = value; }
    float ColorSaturation;

    Vector2 movementDirection;
    public Rigidbody2D rb;
    public CircleCollider2D circleCollider;
    bool canChangeDirection = true;
    public bool canMove = true;
    bool isKnockback;
    bool canAttack = true;
    bool isAttacking = false;
    public VirusData virusData;
    [SerializeField] ParticleSystem fusionParticle;
    public float health;
    float maxHealth;
    MeshRenderer meshRenderer;
    [SerializeField] EnemyMotherDetection normalVirusDetectionRadius;
    [SerializeField] float setDetectionRadius;


    Color defaultColor;


    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float rotationMultiplier = 500f;
    [SerializeField] float weaknessDivider;
    float speedMultiplier = 1.0f;



    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();

        normalVirusDetectionRadius = GetComponentInChildren<EnemyMotherDetection>();
        normalVirusDetectionRadius.gameObject.GetComponent<CircleCollider2D>().radius = setDetectionRadius;

    }

    private void Start()
    {
        UpdateVirusData();
        defaultColor = meshRenderer.material.GetColor("_FresnelColor");
        maxHealth = virusData.Health / 3;
        health = maxHealth;
    }

    private void Update()
    {
        float posX = Mathf.Clamp(transform.position.x, -GameManager.instance.gameArea.x / 2, GameManager.instance.gameArea.x / 2);
        float posY = Mathf.Clamp(transform.position.y, -GameManager.instance.gameArea.y / 2, GameManager.instance.gameArea.y / 2);
        transform.position = new Vector2(posX, posY);

        if (canChangeDirection && canMove)
        {
            StartCoroutine(MoveCharacter());
        }

        if (normalVirusDetectionRadius.normalVirusDetected.Count > 0)
        {
            Debug.DrawRay(transform.position, normalVirusDetectionRadius.normalVirusDetected[0].transform.position - transform.position);
            canMove = false;
            MoveToTarget();
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

        //Rotate text to camera
       


        if (canAttack && normalVirusDetectionRadius.normalVirusDetected.Count != 0 && !isBlocked())
        {
            if (Vector2.Distance(normalVirusDetectionRadius.normalVirusDetected[0].transform.position, transform.position) < 5.5f)
                StartCoroutine(AttackTarget());
        }

        if (isAttacking)
        {
            AttackUpdate();
        }

        ColorOverLife();
    }

    IEnumerator MoveCharacter()
    {
        canChangeDirection = false;
        movementDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        rb.velocity = Vector2.zero;
        rb.AddForce(movementDirection * virusData.Speed * speedMultiplier, ForceMode2D.Impulse);
        yield return new WaitForSeconds(Random.Range(1f, 5f));
        canChangeDirection = true;

    }

    void MoveToTarget()
    {

        movementDirection = (normalVirusDetectionRadius.normalVirusDetected[0].transform.position - transform.position).normalized;

        if (!isKnockback)
        {
            if (!isAttacking)
            {
                if (Vector2.Distance(normalVirusDetectionRadius.normalVirusDetected[0].transform.position, transform.position) > 3.5f)
                {
                    rb.velocity = Vector2.zero;
                    rb.AddForce(movementDirection * virusData.Speed * speedMultiplier, ForceMode2D.Impulse);
                }
                else if (Vector2.Distance(normalVirusDetectionRadius.normalVirusDetected[0].transform.position, transform.position) < 3.5f)
                {
                    rb.velocity = Vector2.zero;
                    rb.AddForce(-movementDirection * virusData.Speed * speedMultiplier, ForceMode2D.Impulse);
                }
            }
        }



    }

    IEnumerator AttackTarget()
    {

        canAttack = false;
        isAttacking = true;

        movementDirection = (normalVirusDetectionRadius.normalVirusDetected[0].transform.position - transform.position).normalized;
        rb.velocity = Vector2.zero;
        rb.AddForce(movementDirection * virusData.Speed * speedMultiplier * 10, ForceMode2D.Impulse); //Impulso
        yield return new WaitForSeconds(.5f); 
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(7);
        canAttack = true;
        isAttacking = false;
    }



    void AttackUpdate()
    {
        if (normalVirusDetectionRadius.normalVirusDetected.Count > 0)
        {
            if (Vector2.Distance(normalVirusDetectionRadius.normalVirusDetected[0].transform.position, transform.position) <= 2.2f)
            {
                isAttacking = false;
                rb.velocity = Vector2.zero;
                normalVirusDetectionRadius.normalVirusDetected[0].GetComponent<IDamageable>().takeDamage(virusData.Damage, 0.5f);
                speedMultiplier = 3;
                Invoke("SetNormalSpeed", .7f);
            }
        }
    }

    void SetNormalSpeed()
    {
        speedMultiplier = 1f;
    }

    bool isBlocked()
    {
        RaycastHit2D[] hits;

        hits = Physics2D.RaycastAll(transform.position, (normalVirusDetectionRadius.normalVirusDetected[0].transform.position - transform.position).normalized, Vector3.Distance(transform.position, normalVirusDetectionRadius.normalVirusDetected[0].transform.position));
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject != gameObject && !hit.collider.gameObject.CompareTag("EnemyMother") && !hit.collider.gameObject.CompareTag("Area") && !hit.collider.gameObject.CompareTag("Virus"))
                {
                    return true;
                }
            }
        }
        return false;

    }

    public void UpdateVirusData()
    {
        //Name
        gameObject.name = virusData.Name;

        //Health
        health = virusData.Health / weaknessDivider;

        //Mesh
        gameObject.GetComponentInChildren<MeshFilter>().mesh = virusData.VirusMesh;

        //Particle
        fusionParticle.Play();

    }


    public void takeDamage(float damage, float force)
    { 
        health -= damage;

        //Debug.Log(health);

        StartCoroutine(ChangeColorDamage());
        StartCoroutine(KnockBack(force));

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

    public IEnumerator KnockBack(float force)
    {
        isKnockback = true;
        rb.AddForce(-movementDirection * (virusData.Speed * speedMultiplier * force), ForceMode2D.Impulse);
        yield return new WaitForSeconds(.2f);
        isKnockback = false;
    }

    public IEnumerator ChangeColorDamage()
    {
        meshRenderer.material.SetColor("_FresnelColor", Color.red);
        yield return new WaitForSeconds(.3f);
        meshRenderer.material.SetColor("_FresnelColor", defaultColor);
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
