using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Bat : MonoBehaviour, IDamageable, IKnockbackable
{

    [Header("Pathfinding")]
    public GameObject player;
    public Transform target;
    public float activateDistance = 50f;
    public float pathUpdateSeconds = 0.5f;
    public float nextWaypointDistance = 3f;

    [Header("Physics")]
    public float speed = 200f;

    [Header("Custom Behavior")]
    public bool followEnabled = true;
    public bool directionLookEnabled = true;

    [Header("Combat")]
    public LayerMask enemyLayers;
    public Collider2D combatCollider;
    public GameObject coinDrop;
    public float attackRange = 0.5f;
    public int attackDamage = 1;
    public int maxHealth = 1;
    public int currentHealth;
    public float knockbackX = 5;
    public float knockbackY = 5;
    public float attackSpeed = 0.1f;
    float nextAttackTime = 0f;
    bool movementControl = true;

    private Path path;
    private int currentWaypoint = 0;
    Seeker seeker;
    Rigidbody2D rb;

    public void Start() 
    {
        player = GameObject.Find ("PlayerCharacter");
        target = player.GetComponent<Transform>();
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

    }
    
    public void Knockback(float knockbackPwrX, float knockbackPwrY, Transform obj)
    {
        StartCoroutine(StartKnockback(knockbackPwrX, knockbackPwrY-10, obj));
    }
    public IEnumerator StartKnockback(float knockbackPwrX, float knockbackPwrY, Transform obj){

        float knockDur = 0.5f;
        float timer = 0;
        
        movementControl = false;
        combatCollider.enabled = false;
        

        while( knockDur > timer ) {
            timer += Time.deltaTime;
            Vector2 direction = (obj.transform.position - this.transform.position).normalized;
            direction.x *= knockbackPwrX;
            direction.y = -0.5f;
            direction.y *= knockbackPwrY;
            rb.AddForce(-direction);
        }

    
        yield return new WaitForSeconds(0.5f); 
        combatCollider.enabled = true;
        movementControl = true;
    }


    public void Damage(int damage) {
        currentHealth -= damage;

        if(currentHealth == 0) {
            GameObject coin = (GameObject)Instantiate(coinDrop, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
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
        if (path == null  || movementControl == false)
        {
            return;
        }


        // Reached end of path
        if (currentWaypoint == path.vectorPath.Count)
        {
            
            return;
        }

         // Attack
        if(Time.time >= nextAttackTime)
        {
            //Make Bat Attack
        }
        //See if colliding with anything

        // Direction Calculation
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

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
