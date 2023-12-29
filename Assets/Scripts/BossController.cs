using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossController : MonoBehaviour
{
    Vector2 movementDirection;
    Vector2 initialMousePosition;
    Vector2 initialObjectPosition;
    public Rigidbody2D rb;
    public CircleCollider2D circleCollider;
    bool canChangeDirection = true;
    bool canIncreaseCoins = true;
    public bool canMove = true;
    public BossData bossData;
    [SerializeField] ParticleSystem fusionParticle;
    [SerializeField] TMP_Text coinsText;
    public int health;


    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float rotationMultiplier = 200f;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();

    }

    private void Start()
    {
        UpdateBossData();
    }

    private void Update()
    {
        if (canChangeDirection && canMove)
        {
            StartCoroutine(MoveCharacter());
        }



        //Rotation
        // Generate random rotation for each axis
        Quaternion randomRotation = Quaternion.Euler(
            movementDirection.x * rotationMultiplier, // Random rotation on X axis
            movementDirection.y * rotationMultiplier, // Random rotation on Y axis
            movementDirection.x * rotationMultiplier  // Random rotation on Z axis
        );

        // Apply the rotation gradually over time
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            randomRotation,
            rotationSpeed * Time.deltaTime
        );

    }

    IEnumerator MoveCharacter()
    {
        canChangeDirection = false;
        movementDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        rb.velocity = Vector2.zero;
        rb.AddForce(movementDirection * bossData.Speed, ForceMode2D.Impulse);
        yield return new WaitForSeconds(Random.Range(1f, 5f));
        canChangeDirection = true;

    }


    public void UpdateBossData()
    {
        //Name
        gameObject.name = bossData.Name;

        //Health
        health = bossData.Health;

        //Icon
        //gameObject.GetComponent<SpriteRenderer>().sprite = virusData.Icon;

        //Mesh
        gameObject.GetComponentInChildren<MeshFilter>().mesh = bossData.VirusMesh;

        //Particle
        fusionParticle.Play();

    }
}
