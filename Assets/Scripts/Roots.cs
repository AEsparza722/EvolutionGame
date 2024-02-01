using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roots : MonoBehaviour
{
    float speed = 10f;
    float rotationSpeed = 200f;
    float changeDirection = 1f;
    float timeSinceLastChangeDirection = 0;
    bool disolve = false;
    float disolveTimeStart = 0;
    float currentDisolveTime = 0;
    float disolveDuration = 5;
    Rigidbody2D rb;
    Vector2 targetDirection;
    LineRenderer lineRenderer;
    List<Vector3> lineRendererPosition = new List<Vector3>();

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = rb.GetComponent<LineRenderer>();
        GenerateRandomDirection();
        currentDisolveTime = 0f;
        disolveTimeStart = .3f;
    }

    private void Update()
    {
        Vector2 currentDirection = rb.velocity.normalized;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        float currentAngle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
        float deltaAngle = Mathf.MoveTowardsAngle(currentAngle, angle, rotationSpeed * Time.deltaTime);
        Vector2 newDirection = new Vector2(Mathf.Cos(deltaAngle * Mathf.Deg2Rad), Mathf.Sin(deltaAngle * Mathf.Deg2Rad));
        transform.rotation = Quaternion.Euler(0,0,angle);
        rb.velocity = newDirection * speed;
        timeSinceLastChangeDirection += Time.deltaTime;

        if (timeSinceLastChangeDirection >= changeDirection)
        {
            GenerateRandomDirection();
            timeSinceLastChangeDirection = 0f;
        }

        UpdateLineRenderer();
        currentDisolveTime += Time.deltaTime;
        if (currentDisolveTime >= disolveTimeStart && !disolve)
        {
            disolve = true;
            StartCoroutine(DisolveLineRenderer(1));
        }
               
    }

    void RestartRoot()
    {
        lineRenderer.material.SetFloat("_Disolve", 1f);
        transform.position = transform.parent.position;
        currentDisolveTime = 0f;
        disolve = false;
        lineRenderer.enabled = true;
        lineRendererPosition.Clear();
        lineRenderer.positionCount = 0;
    }

    void GenerateRandomDirection()
    {
        targetDirection = Random.insideUnitCircle.normalized;
    }

    void UpdateLineRenderer()
    {
        lineRendererPosition.Add(transform.position);
        lineRenderer.positionCount = lineRendererPosition.Count;
        lineRenderer.SetPositions(lineRendererPosition.ToArray());
    }

    IEnumerator DisolveLineRenderer(float time)
    {
        float startTime = Time.time;
        while (Time.time < startTime + time)
        {
            float normalizedTime = (Time.time - startTime) / time;
            lineRenderer.material.SetFloat("_Disolve", disolveDuration - normalizedTime);
            yield return null;
        }
        lineRenderer.enabled = false;
        Invoke("RestartRoot", 1);
    }

}
