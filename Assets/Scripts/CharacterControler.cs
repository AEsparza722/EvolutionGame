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
    bool isKnockback;
    public VirusData virusData;
    [SerializeField] ParticleSystem fusionParticle;
    [SerializeField] TMP_Text coinsText;
    int health;
    MeshRenderer meshRenderer;

 
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float rotationMultiplier = 500f;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();    
        meshRenderer = GetComponentInChildren<MeshRenderer>();
                
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

        if (BossSystem.instance.currentBoss != null)
        {
            canMove = false;
            MoveToBoss();
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
         coinsText.transform.LookAt(Camera.main.transform.position);


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

    void MoveToBoss()
    {
        
            movementDirection = (BossSystem.instance.currentBoss.transform.position - transform.position).normalized;
            
        if (!isKnockback)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(movementDirection * virusData.Speed, ForceMode2D.Impulse);
        }

    }

    IEnumerator DropCoins ()
    {
        canIncreaseCoins = false;
        yield return new WaitForSeconds(5);
        GameManager.instance.UpdateCoins(virusData.Coins);
        BossSystem.instance.IncreaseScore(virusData.Coins);
        coinsText.text = "+ " + virusData.Coins.ToString();
        TextAnimation();
        canIncreaseCoins = true;

    //Attack boss
        if (BossSystem.instance.currentBoss != null)
        {
            if ((BossSystem.instance.currentBoss.transform.position - transform.position).magnitude <= 3f)
            {
                BossSystem.instance.takeDamage(virusData.Damage);                
            }
        }

    }

    void TextAnimation()
    {
        LeanTween.scale(coinsText.rectTransform, new Vector3(-1, 1, 1), .3f).setEase(LeanTweenType.easeInOutSine).setOnComplete(() => { LeanTween.scale(coinsText.rectTransform, new Vector3(0, 0, 0), .3f).setEase(LeanTweenType.easeInOutSine).setDelay(.3f); });
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
        gameObject.GetComponentInChildren<MeshFilter>().mesh = virusData.VirusMesh;

        //Particle
        fusionParticle.Play();

    }

    private void OnMouseDrag()
    {        
        Vector2 mouseDrag = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseDrag.x = Mathf.Clamp(mouseDrag.x, -4.45f, 4.45f);
        mouseDrag.y = Mathf.Clamp(mouseDrag.y, -8.4f, 8.4f);
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

    public void takeDamage(int damage, float force)
    {
        health -= damage;
        
        Debug.Log(health);

        StartCoroutine(ChangeColorDamage());
        StartCoroutine(KnockBack(force));

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator ChangeColorDamage()
    {
        Color defaultColor = meshRenderer.material.GetColor("_FresnelColor");

        meshRenderer.material.SetColor("_FresnelColor", Color.red);
        yield return new WaitForSeconds(.5f);

        meshRenderer.material.SetColor("_FresnelColor", defaultColor);
    }

    IEnumerator KnockBack (float force)
    {
        isKnockback = true;
        rb.AddForce(-movementDirection * (virusData.Speed * force), ForceMode2D.Impulse);
        yield return new WaitForSeconds(.2f);
        isKnockback = false;
    }
}
