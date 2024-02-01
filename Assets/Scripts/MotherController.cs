using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MotherController : MonoBehaviour
{
    Vector2 movementDirection;
    bool canChangeDirection = true;
    bool canMove = false;
    CircleCollider2D circleCollider;
    Rigidbody2D rb;
    MeshRenderer meshRenderer;
    [SerializeField] VirusAreaDetection virusDetectionRadius;
    [SerializeField] float setDetectionRadius;
    [SerializeField] GameObject root;

    [SerializeField] float speed;

    
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float rotationMultiplier = 500f;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();

        virusDetectionRadius = GetComponentInChildren<VirusAreaDetection>();
        virusDetectionRadius.gameObject.GetComponent<CircleCollider2D>().radius = setDetectionRadius;

    }

    private void Start()
    {
        InstantiateRoots();
    }

    private void Update()
    {
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

    void InstantiateRoots()
    {
        for (int i = 0; i < 5; i++)
        {
            Instantiate(root, transform.position, Quaternion.identity, transform);
        }
    }

}
