using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;

public class CharacterControler : MonoBehaviour, IDamageable
{
    float IDamageable.health { get => health; set => health = value; }
    public float colorSaturation { get => ColorSaturation; set => ColorSaturation = value; }
    float ColorSaturation;

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
    bool isAttacking = false;
    public bool isMoveSelection = false;
    public VirusData virusData;
    [SerializeField] ParticleSystem fusionParticle;
    [SerializeField] TMP_Text coinsText;
    public float health;
    MeshRenderer meshRenderer;
    [SerializeField] VirusAreaDetection virusDetectionRadius;
    [SerializeField] float setDetectionRadius;
    [SerializeField] TMP_Text maxLevelText;
    Vector2 clickPosition;
    

    Color defaultColor;


    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float rotationMultiplier = 500f;
    float speedMultiplier = 1.0f;

    

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
        maxLevelText = GameObject.FindGameObjectWithTag("MaxLevelReachedText").GetComponent<TMP_Text>();
    }

    private void Update()
    {
        float posX = Mathf.Clamp(transform.position.x, -GameManager.instance.gameArea.x / 2, GameManager.instance.gameArea.x / 2);
        float posY = Mathf.Clamp(transform.position.y, -GameManager.instance.gameArea.y / 2, GameManager.instance.gameArea.y / 2);
        transform.position = new Vector2(posX, posY);

        if (canChangeDirection && canMove && !isMoveSelection)
        {
            StartCoroutine(MoveCharacter());
        }

        if(virusDetectionRadius.bossDetected.Count > 0 && !isMagnet && !isMoveSelection)
        {
            Debug.DrawRay(transform.position, virusDetectionRadius.GetNearbyEnemy().transform.position - transform.position);            
            canMove = false;
            MoveToBoss();
        }

        if (canIncreaseCoins)
        {
            StartCoroutine(DropCoins());
        }

        if (isMoveSelection)
        {
            movementDirection = (clickPosition - (Vector2)transform.position).normalized;
            rb.velocity = Vector2.zero;
            rb.AddForce(movementDirection * virusData.Speed * speedMultiplier, ForceMode2D.Impulse);

            if(Vector2.Distance(transform.position, clickPosition) < 3)
            {
                isMoveSelection = false;
            }
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


        if (canAttack && virusDetectionRadius.bossDetected.Count != 0 && !isBlocked())
        {
            if (Vector2.Distance(virusDetectionRadius.GetNearbyEnemy().transform.position, transform.position) < 5.5f)
                StartCoroutine(AttackBoss());
        }

        if (GameManager.instance.isGameOver)
        {
            Destroy(gameObject);
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
        movementDirection = new Vector2(Random.Range(-1f,1f), Random.Range(-1f, 1f));
        rb.velocity = Vector2.zero;
        rb.AddForce(movementDirection*virusData.Speed * speedMultiplier, ForceMode2D.Impulse);
        yield return new WaitForSeconds(Random.Range(1f,5f));
        canChangeDirection = true;
                
    }

    void MoveToBoss()
    {
        
            movementDirection = (virusDetectionRadius.GetNearbyEnemy().transform.position - transform.position).normalized;
            
        if (!isKnockback)
        {
            if (!isMagnet && !isAttacking)
            {
                if (Vector2.Distance(virusDetectionRadius.GetNearbyEnemy().transform.position, transform.position) > 3.5f)
                {
                    rb.velocity = Vector2.zero;
                    rb.AddForce(movementDirection * virusData.Speed * speedMultiplier, ForceMode2D.Impulse);
                }
                else if(Vector2.Distance(virusDetectionRadius.GetNearbyEnemy().transform.position, transform.position) < 3.5f)
                {
                    rb.velocity = Vector2.zero;
                    rb.AddForce(-movementDirection * virusData.Speed * speedMultiplier, ForceMode2D.Impulse);
                }                               
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
        isAttacking = true;

        movementDirection = (virusDetectionRadius.GetNearbyEnemy().transform.position - transform.position).normalized;
        rb.velocity = Vector2.zero;
        rb.AddForce(movementDirection * virusData.Speed * speedMultiplier * 10, ForceMode2D.Impulse); //Impulso
        yield return new WaitForSeconds(.5f);
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(5);
        canAttack = true;
        isAttacking = false;               
    }

    

    void AttackUpdate()
    {
        if (virusDetectionRadius.bossDetected.Count > 0)
        {
            if (Vector2.Distance(virusDetectionRadius.GetNearbyEnemy().transform.position, transform.position) <= 2.2f)
            {
                isAttacking = false;
                rb.velocity = Vector2.zero;
                virusDetectionRadius.GetNearbyEnemy().GetComponent<IDamageable>().takeDamage(virusData.Damage, 0.5f);
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

        hits = Physics2D.RaycastAll(transform.position, (virusDetectionRadius.GetNearbyEnemy().transform.position - transform.position).normalized, Vector3.Distance(transform.position, virusDetectionRadius.GetNearbyEnemy().transform.position));
        foreach (RaycastHit2D hit in hits)
        {
            if(hit.collider != null) 
            {
                if (hit.collider.gameObject != gameObject && !hit.collider.gameObject.CompareTag("Boss") && !hit.collider.gameObject.CompareTag("Area") && !hit.collider.gameObject.CompareTag("EnemyMother") && !hit.collider.gameObject.CompareTag("EnemyVirus"))
                {                    
                    return true;                    
                }
            }         
        }        
        return false;
        
    }

    void TextAnimation()
    {
        LeanTween.scale(coinsText.rectTransform, new Vector3(-1, 1, 1), .3f).setEase(LeanTweenType.easeInOutSine).setOnComplete(() => { LeanTween.scale(coinsText.rectTransform, new Vector3(0, 0, 0), .3f).setEase(LeanTweenType.easeInOutSine).setDelay(.3f); });
    }

    void TextMaxLevelReached()
    {
        LeanTween.scale(maxLevelText.rectTransform, new Vector3(1,1,1), .3f).setEase(LeanTweenType.easeInOutSine).setOnComplete(() => { LeanTween.scale(maxLevelText.rectTransform, new Vector3(0, 0, 0), .3f).setEase(LeanTweenType.easeInOutSine).setDelay(1f); });
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
        if (!isMagnet)
        {
            Vector2 mouseDrag = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseDrag.x = Mathf.Clamp(mouseDrag.x, -GameManager.instance.gameArea.x - 5, GameManager.instance.gameArea.x - 5);
            mouseDrag.y = Mathf.Clamp(mouseDrag.y, -GameManager.instance.gameArea.y - 5, GameManager.instance.gameArea.y - 5);
            Vector2 mouseOffset = mouseDrag - initialMousePosition;
            transform.position = initialObjectPosition + mouseOffset;

        }
        else { return; 
        }
        
    }
    private void OnMouseDown()
    {
        GameManager.instance.isDraggingVirus = true;
        initialMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        initialObjectPosition = transform.position;
        circleCollider.radius /= 2;
    }
    private void OnMouseUp()
    {
        FusionVirus();                
        FeedMother();
        GameManager.instance.isDraggingVirus = false;
    }

    void FusionVirus()
    {
        circleCollider.radius *= 2;
        circleCollider.isTrigger = false;

        if (virusData.VirusLevel < GameManager.instance.maxVirusLevel)
        {
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
        else
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f);

            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Virus") && collider.gameObject != gameObject)
                {
                    if (collider.gameObject.GetComponent<CharacterControler>().virusData.VirusLevel == virusData.VirusLevel)
                    {
                        TextMaxLevelReached();
                        return;

                    }
                }
            }

            
        }
        
    }

    void FeedMother()
    {
        circleCollider.radius *= 2;
        circleCollider.isTrigger = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Protect") && MotherController.instance.health < MotherController.instance.maxHealth)
            {
                MotherController.instance.Heal( virusData.VirusLevel * virusData.VirusLevel * 10);
                Destroy(gameObject);
            }
        }
        circleCollider.radius /= 2;
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
        ColorSaturation = (health / virusData.Health) * 1;

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

    public void MoveSelection(Vector2 clickPosition)
    {
        isMoveSelection = true;        
        this.clickPosition = clickPosition;
    }
}
