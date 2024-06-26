using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BossController : MonoBehaviour, IIndicator, IDamageable
{
    public Color color { get => indicatorColor; set => indicatorColor = value; }
    public bool isActive { get => indicatorActive; set => indicatorActive = value; }

    public float colorSaturation { get => ColorSaturation; set => ColorSaturation = value; }
    float ColorSaturation;

    float IDamageable.health { get => health; set => health = value; }

    [Header("Components")]
    public Rigidbody2D rb;
    public CircleCollider2D circleCollider;
    public BossDetectionRadius bossDetectionRadius;
    [SerializeField] float setDetectionRadius;
    [SerializeField] Color indicatorColor = Color.red;
    [SerializeField] GameObject indicatorArrowPrefab;
    [SerializeField] GameObject basicAttackPrefab;
    public GameObject indicatorArrow;
    Animator animator;
    MeshRenderer meshRenderer;


    

    [Header("Behaviours")]
    Vector2 movementDirection;    
    protected bool canChangeDirection = true;
    public bool canMove = true;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float rotationMultiplier = 200f;
    bool indicatorActive = true;
        

    [Header("Attack")]
    [SerializeField] protected float attackRadius = 3f;
    [SerializeField] protected float cooldownBasicAttack = 3f;
    protected bool canAttack = true;
    protected bool isAttacking = false;
    protected List<GameObject> viruses = new List<GameObject>();
    GameObject virusFocus;

    [Header("Stats")]
    public float damage;
    public float health;
    public float speed;
    public float maxHealth;

    [Header("Effects")]
    [SerializeField] ParticleSystem fusionParticle;

    protected void InitializeBoss()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        bossDetectionRadius = GetComponentInChildren<BossDetectionRadius>();
        bossDetectionRadius.gameObject.GetComponent<CircleCollider2D>().radius = setDetectionRadius;
        indicatorArrow = Instantiate(indicatorArrowPrefab);
        indicatorArrow.GetComponent<SpriteRenderer>().color = indicatorColor;
        health = maxHealth;
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public virtual void UpdateBoss()
    {
        float posX = Mathf.Clamp(transform.position.x, -GameManager.instance.gameArea.x / 2, GameManager.instance.gameArea.x / 2);
        float posY = Mathf.Clamp(transform.position.y, -GameManager.instance.gameArea.y / 2, GameManager.instance.gameArea.y / 2);
        transform.position = new Vector2(posX, posY);

        if (canChangeDirection && canMove)
        {
            StartCoroutine(MoveCharacter());
        }

        if (bossDetectionRadius.virusDetected.Count > 0)
        {
            canMove = false;
            ChaseVirus();
        }
        else
        {
            canMove = true;
        }

        if (canAttack)
        {
            StartCoroutine(BasicAttack());
        }

        Detect();

        ColorOverLife();

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

        if (GameManager.instance.isGameOver)
        {
            StopAllCoroutines();
        }
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

    void ChaseVirus()
    {
        movementDirection = (bossDetectionRadius.virusDetected[0].transform.position - transform.position).normalized;
        rb.velocity = Vector2.zero;
        rb.AddForce(movementDirection * speed, ForceMode2D.Impulse);
    }

    IEnumerator BasicAttack()
    {

        canAttack = false;
        viruses = GetVirusInRange(attackRadius);

        if (virusFocus == null && viruses.Count > 0)
        {
            virusFocus = viruses[0];
            AttackVirus();
        }
        else if (viruses.Count > 0 && virusFocus != null)
        {
            if (viruses.Contains(virusFocus))
            {
                AttackVirus();
            }
            else //Virusfocus no null pero no encontro al virus
            {
                virusFocus = viruses[0];
                AttackVirus();
            }
        }
        else //No encontro virusfocus
        {
            virusFocus = null;
        }

        yield return new WaitForSeconds(cooldownBasicAttack);
        canAttack = true;
    }

    protected List<GameObject> GetVirusInRange(float range)
    {
        List<GameObject> virusInRange = new List<GameObject>();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, range);
        virusInRange.Clear();        

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.CompareTag("Virus") || colliders[i].gameObject.CompareTag("Protect"))
            {
                virusInRange.Add(colliders[i].gameObject);
            }
        } //Agregamos todos los virus en la lista
        return virusInRange;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
        
    }

    void AttackVirus()
    {        
        GameObject basicAttack = Instantiate(basicAttackPrefab, transform.position, Quaternion.identity);
        basicAttack.GetComponent<BossBasicAttack>().Initialize(basicAttack.transform, virusFocus.transform, 5f, 10f, damage);

        if (virusFocus.GetComponent<IDamageable>().health <= 0)
        {
            StartCoroutine(PostProcess.instance.DamagePostProcess(.5f, transform.position)); //DamagePost process
            ShakeCamera.instance.ShakeCam(1, .3f, transform.position); //Shake camera, cambiar a viruses
        }

    } 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spawner"))
        {
            StartCoroutine(SpawnVirusSystem.instance.ReturnToPool(0, collision.gameObject));
        }
    }

    public void Detect()
    {
        if (indicatorActive)
        {

            Vector2 direction = transform.position - indicatorArrow.transform.position;
            Vector2 targetViewportPosition = Camera.main.WorldToViewportPoint(transform.position);
            float distance = direction.magnitude;
            float normalizedDistance = 1.5f - (distance / 50f) * 2f;



            if (targetViewportPosition.x > 0f && targetViewportPosition.x < 1f && targetViewportPosition.y > 0f && targetViewportPosition.y < 1f)
            {
                indicatorArrow.SetActive(false);
            }
            else
            {
                indicatorArrow.SetActive(true);

                //Position
                Vector3 screenEgde = Camera.main.ViewportToWorldPoint(new Vector3(Mathf.Clamp(targetViewportPosition.x, 0.05f, 0.95f), Mathf.Clamp(targetViewportPosition.y, 0.1f, 0.9f), Camera.main.nearClipPlane));
                indicatorArrow.transform.position = screenEgde;

                //Rotation
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                indicatorArrow.transform.rotation = Quaternion.Euler(0, 0, angle - 90);

             
                indicatorArrow.transform.localScale = new Vector3(normalizedDistance, normalizedDistance, indicatorArrow.transform.localScale.z);

            }
        }
    }


    public void takeDamage(float damage, float force)
    {        
        health -= damage;
        animator.SetTrigger("hit");

        //Debug.Log(bossController.health);
        if (health <= 0)
        {
            Destroy(indicatorArrow);
            BossSystem.instance.BossKilled();
            PostProcess.instance.PostProcessDefault();
            Destroy(gameObject);            
        }
    }

    void OnMouseDown()
    {
        if (UpgradeController.instance.canClickDamage)
        {
            takeDamage(UpgradeController.instance.ClickDamage.CurrentEffect * UpgradeController.instance.ClickDamage.Level,0);
            Debug.Log(UpgradeController.instance.ClickDamage.CurrentEffect * UpgradeController.instance.ClickDamage.Level);
        }
    }
       

    public IEnumerator takeDamageOverTime(float damage, float times, float seconds)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator KnockBack(float force)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator ChangeColorDamage()
    {
        throw new System.NotImplementedException();
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
