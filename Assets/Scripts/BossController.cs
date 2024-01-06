using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossController : MonoBehaviour
{
    
    

    [Header("Components")]
    public Rigidbody2D rb;
    public CircleCollider2D circleCollider;

    [Header("Behaviours")]
    Vector2 movementDirection;    
    protected bool canChangeDirection = true;
    public bool canMove = true;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float rotationMultiplier = 200f;

    [Header("Attack")]
    [SerializeField] protected float attackRadius = 3f;
    [SerializeField] protected float cooldownBasicAttack = 3f;
    protected bool canAttack = true;
    protected List<GameObject> viruses = new List<GameObject>();
    GameObject virusFocus;

    [Header("Stats")]
    public int damage;
    public int health;
    public float speed;

    [Header("Effects")]
    [SerializeField] ParticleSystem fusionParticle;    
    
    protected void InitializeBoss()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected void UpdateBoss()
    {
        if (canChangeDirection && canMove)
        {
            StartCoroutine(MoveCharacter());
        }

        if (canAttack)
        {
            StartCoroutine(BasicAttack());
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
        rb.AddForce(movementDirection * speed, ForceMode2D.Impulse);
        yield return new WaitForSeconds(Random.Range(1f, 5f));
        canChangeDirection = true;

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
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRadius);
        virusInRange.Clear();        

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.CompareTag("Virus"))
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
        virusFocus.GetComponent<CharacterControler>().takeDamage(damage, 3.5f);
        StartCoroutine(PostProcess.instance.DamagePostProcess(.5f)); //DamagePost process
        ShakeCamera.instance.ShakeCam(2, .5f); //Shake camera, cambiar a viruses
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spawner"))
        {
            Destroy(collision.gameObject);
        }
    }
}
