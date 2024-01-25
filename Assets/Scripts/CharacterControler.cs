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
    public bool isMagnet;
    bool canAttack = true;
    public VirusData virusData;
    [SerializeField] ParticleSystem fusionParticle;
    [SerializeField] TMP_Text coinsText;
    public float health;
    MeshRenderer meshRenderer;
    [SerializeField] VirusAreaDetection virusDetectionRadius;
    [SerializeField] float setDetectionRadius;
    

    Color defaultColor;


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
        UpdateVirusData();
        defaultColor = meshRenderer.material.GetColor("_FresnelColor");
    }

    private void Update()
    {
        if (canChangeDirection && canMove)
        {
            StartCoroutine(MoveCharacter());
        }

        if(virusDetectionRadius.bossDetected.Count > 0 && !isMagnet)
        {
            canMove = false;
            MoveToBoss();
        }

        if (canIncreaseCoins)
        {
            StartCoroutine(DropCoins());
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

        if (canAttack)
        {
            StartCoroutine(AttackBoss());
        }
        
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
        
            movementDirection = (virusDetectionRadius.bossDetected[0].transform.position - transform.position).normalized;
            
        if (!isKnockback)
        {
            if (!isMagnet)
            {
                rb.velocity = Vector2.zero;
                rb.AddForce(movementDirection * virusData.Speed, ForceMode2D.Impulse);
            }
            
            
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

      

    }

    IEnumerator AttackBoss()
    {
        canAttack = false;
        List<GameObject> bossesDetected = new List<GameObject>();
        Collider2D[] bossColliders = Physics2D.OverlapCircleAll(transform.position, virusData.AttackRadius);
        foreach (Collider2D collider in bossColliders)
        {
            if (collider.gameObject.CompareTag("Boss"))
            {
                bossesDetected.Add(collider.gameObject);                
            }
        }
        if(bossesDetected.Count > 0)
        {
            bossesDetected[0].GetComponent<BossController>().takeDamage(virusData.Damage);
            Debug.Log("SI");
        }
        yield return new WaitForSeconds(5);
        canAttack = true;
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
        mouseDrag.x = Mathf.Clamp(mouseDrag.x, -GameManager.instance.gameArea.x -5, GameManager.instance.gameArea.x -5);
        mouseDrag.y = Mathf.Clamp(mouseDrag.y, -GameManager.instance.gameArea.y - 5, GameManager.instance.gameArea.y - 5);
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

    public void takeDamage(float damage, float force)
    {
        if (isMagnet)
        {
            return;
        }
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

    IEnumerator ChangeColorDamage()
    {
        meshRenderer.material.SetColor("_FresnelColor", Color.red);
        yield return new WaitForSeconds(.3f);
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
