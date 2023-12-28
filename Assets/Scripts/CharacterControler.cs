using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterControler : MonoBehaviour
{
    Vector2 movementDirection;
    Vector2 initialMousePosition;
    Vector2 initialObjectPosition;
    public Rigidbody2D rb;
    public CircleCollider2D circleCollider;
    bool canChangeDirection = true;
    bool canIncreaseCoins = true;
    public bool canMove = true;
    public VirusData virusData;
    [SerializeField] ParticleSystem fusionParticle;
    [SerializeField] TMP_Text coinsText;
    int health;

    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float rotationMultiplier = 200f;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
                
    }

    private void Start()
    {
        UpdateVirusData();
    }

    private void Update()
    {
        if (canChangeDirection && canMove)
        {
            StartCoroutine(MoveCharacter());
        }

        if (canIncreaseCoins)
        {
            StartCoroutine(DropCoins());
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
        movementDirection = new Vector2(Random.Range(-1f,1f), Random.Range(-1f, 1f));
        rb.velocity = Vector2.zero;
        rb.AddForce(movementDirection*virusData.Speed,ForceMode2D.Impulse);
        yield return new WaitForSeconds(Random.Range(1f,5f));
        canChangeDirection = true;
                
    }

    IEnumerator DropCoins ()
    {
        canIncreaseCoins = false;                
        yield return new WaitForSeconds(5);
        GameManager.instance.UpdateCoins(virusData.Coins);
        coinsText.text = "+ " + virusData.Coins.ToString();
        TextAnimation();
        canIncreaseCoins = true;
    }

    void TextAnimation()
    {
        LeanTween.scale(coinsText.rectTransform, new Vector3(1, 1, 1), .3f).setEase(LeanTweenType.easeInOutSine).setOnComplete(() => { LeanTween.scale(coinsText.rectTransform, new Vector3(0, 0, 0), .3f).setEase(LeanTweenType.easeInOutSine).setDelay(.3f); });
    }
 
    public void UpdateVirusData()
    {
        //Name
        gameObject.name = virusData.Name;

        //Health
        health = virusData.Health;

        //Icon
        //gameObject.GetComponent<SpriteRenderer>().sprite = virusData.Icon;

        //Mesh
        gameObject.GetComponent<MeshFilter>().mesh = virusData.VirusMesh;

        //Particle
        fusionParticle.Play();

    }

    private void OnMouseDrag()
    {        
        Vector2 mouseDrag = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseDrag.x = Mathf.Clamp(mouseDrag.x, -9f, 9f);
        mouseDrag.y = Mathf.Clamp(mouseDrag.y, -5.5f, 5.5f);
        Vector2 mouseOffset = mouseDrag - initialMousePosition;
        transform.position = initialObjectPosition + mouseOffset;           
    }
    private void OnMouseDown()
    {
        initialMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        initialObjectPosition = transform.position;
        circleCollider.radius /= 2;
    }
    private void OnMouseUp()
    {
        FusionVirus();
    }

    void FusionVirus()
    {
        circleCollider.radius *= 2;
        circleCollider.isTrigger = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Virus") && collider.gameObject != gameObject)
            {
                if (collider.gameObject.GetComponent<CharacterControler>().virusData.VirusLevel == virusData.VirusLevel)
                {
                    Destroy(collider.gameObject);
                    virusData = VirusManager.instance.NextVirus(virusData);
                    UpdateVirusData();
                    

                    return;

                }

            }

        }


    }
}
