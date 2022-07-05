using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class SlimeAI : MonoBehaviour
{

    [Header("Pathfinding")]
    public Transform target;
    public float activateDistance = 50f;
    public float pathUpdateSeconds = 0.5f;

    [Header("Physics")]
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public float jumpNodeHeightRequirement = 0.8f;
    public float jumpModifier = 0.3f;
    public float JumpCheckOffset = 0.1f;

    [Header("Custom Behavior")]
    public bool followEnabled = true;
    public bool jumpEnabled = true;
    public bool directionLookEnabled = true;
    public float attackModifier = 0.01f;

    [Header("Combat")]
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public float attackRange = 0.5f;
    public int attackDamage = 2;
    public int maxHealth = 2;
    public int currentHealth;
    public float knockback = 5;

    private Path path;
    private int currentWaypoint = 0;
    bool isGrounded = false;
    Seeker seeker;
    Rigidbody2D rb;

    public void Start() 
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;

        
        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

   

    private void FixedUpdate()
    {
        if (TargetInDistance() && followEnabled)
        {
            PathFollow();
        }

        Attack();
    }

    private void OnDrawGizmosSelected()
    {
        if(attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

     void Attack()
    {
        // Deteck enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Damage them
        foreach(Collider2D enemyCollider in hitEnemies)
        {
            if(enemyCollider.isTrigger)
            {
                PlayerMovement enemy = enemyCollider.transform.parent.gameObject.GetComponent<PlayerMovement>();
                
                StartCoroutine(enemy.Knockback(0.5f, 10f, 10f, this.transform));

                enemyCollider.transform.parent.gameObject.GetComponent<Player>().TakeDamage(attackDamage);
            }
        }
    }

    public void TakeDamage(int damage) {
        currentHealth -= damage;

        if(currentHealth == 0)
            Destroy(gameObject);
    }

    private void UpdatePath()
    {
        if (followEnabled && TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);

        }
    }

    private void PathFollow()
    {
        if (path == null)
        {
            return;
        }


        // Reached end of path
        if (currentWaypoint == path.vectorPath.Count)
        {
            
            return;
        }

         // Attack
            if (isGrounded && (path.vectorPath.Count <= 6 && path.vectorPath.Count >= 3) && rb.velocity.y == 0 && !(target.position.y -1f > rb.transform.position.y))
            {
                rb.AddForce(Vector2.up * speed * jumpModifier);
            }

        //See if colliding with anything
        isGrounded = Physics2D.Raycast(transform.position, -Vector3.up, GetComponent<Collider2D>().bounds.extents.y + JumpCheckOffset);

        // Direction Calculation
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        // Jump
        if (jumpEnabled && isGrounded)
        {
            if (target.position.y - 1f > rb.transform.position.y && rb.velocity.y == 0 && path.path.Count < 20)
            {
                rb.AddForce(Vector2.up * speed * jumpModifier);
            }
        }
        
       

        // Movement
        rb.AddForce(force);

        // Next Waypoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        // Direction Graphics Handling
        if (directionLookEnabled)
        {
            if (rb.velocity.x > 0.05f)
            {
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (rb.velocity.x < -0.05f)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    private bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, target.transform.position) < activateDistance;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
 
}
