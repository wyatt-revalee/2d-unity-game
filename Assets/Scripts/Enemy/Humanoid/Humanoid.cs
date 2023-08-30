using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using TMPro;
using UnityEngine.UI;
public class Humanoid : MonoBehaviour, IDamageable, IKnockbackable
{

    [Header("Pathfinding")]
    public GameObject player;
    public Transform target;
    public float activateDistance = 50f;
    public float pathUpdateSeconds = 0.5f;
    public float nextWaypointDistance = 3f;

    [Header("Physics")]
    private float speed = 800f;
    private float jumpModifier = 0.3f;
    private float JumpCheckOffset = 0.1f;

    [Header("Custom Behavior")]
    public bool followEnabled = true;
    public bool jumpEnabled = true;
    public bool directionLookEnabled = true;
    public int coinAmount;

    [Header("Combat")]
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public Collider2D combatCollider;
    public GameObject coinDrop;
    public HealthBar healthBar;
    public GameObject healthBarVisual;
    private int healthBarTime = 0;
    public int attackDamage;
    private int maxHealth;
    private int currentHealth;
    public float knockbackX = 5;
    public float knockbackY = 5;
    bool movementControl = true;

    public SpriteRenderer spriteRen;
    GameObject manager;
    ManageLevels managerScript;
    public Transform sprite;
    private Path path;
    private int currentWaypoint = 0;
    bool isGrounded = false;
    Seeker seeker;
    Rigidbody2D rb;

    public Dictionary<string, InventoryItem> buffDict;
    public List<InventoryItem> buffInv { get; private set; }
    public GameObject BuffBar;
    public GameObject buffSlot;

    public void Start() 
    {
        manager = GameObject.Find("Manager");
        managerScript = manager.GetComponent<ManageLevels>();
        if(managerScript.world == 7)
            spriteRen.color = managerScript.worldColors[UnityEngine.Random.Range(1, 7)];
        else
            spriteRen.color = managerScript.worldColors[managerScript.world];
        player = GameObject.Find ("PlayerCharacter");
        target = player.GetComponent<Transform>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        maxHealth = 8*managerScript.world;
        attackDamage = 2*managerScript.world;
        coinAmount = 4;

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        
        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);

        buffInv = new List<InventoryItem>();
        buffDict = new Dictionary<string, InventoryItem>();
        StartCoroutine(CallBuffUpdate());
    }

   

    private void FixedUpdate()
    {
        UpdateBuffUI();
        if (TargetInDistance() && followEnabled)
        {
            PathFollow();
        }

    }

    // Buff Stuff
    IEnumerator CallBuffUpdate()
    {
        List<InventoryItem> tempBuffs = new List<InventoryItem>(buffInv);
        foreach (InventoryItem i in tempBuffs)
        {
            if (i.itemBuffTime > 0)
            {
                i.data.item.UpdateEnemy(this, i.stackSize);
                i.DecBuffTime();
            }
            else
            {
                RemoveBuff(i.data);
            }
        }

        yield return new WaitForSeconds(1f);
        StartCoroutine(CallBuffUpdate());
    }
    public void AddBuff(InventoryItemData itemData)
    {

        if (buffDict.TryGetValue(itemData.id, out InventoryItem value))
        {
            value.AddToStack();
        }
        else
        {
            InventoryItem buffToAdd = new InventoryItem(itemData);
            buffInv.Add(buffToAdd);
            buffDict.Add(itemData.id, buffToAdd);

        }
    }

    public void RemoveBuff(InventoryItemData itemData)
    {
        if (buffDict.TryGetValue(itemData.id, out InventoryItem value))
        {
            value.RemoveFromStack();

            if (value.stackSize == 0)
            {
                buffInv.Remove(value);
                buffDict.Remove(itemData.id);
            }
        }

    }

    public void UpdateBuffUI()
    {
        foreach (Transform child in BuffBar.transform)
        {
            Destroy(child.gameObject);
        }
        // Debug.Log("Drawing Inventory");
        foreach (InventoryItem buffItem in buffInv)
        {
            if (buffItem.stackSize > 0)
            {
                GameObject newBuff = Instantiate(buffSlot, BuffBar.transform, false);

                var image = newBuff.transform.GetChild(0).gameObject.GetComponent<Image>();
                image.sprite = buffItem.data.icon;

                var buffStack = newBuff.transform.GetChild(1).gameObject.GetComponent<TMP_Text>();
                buffStack.text = buffItem.stackSize.ToString();

            }
        }
    }

    public void Knockback(float knockbackPwrX, float knockbackPwrY, Transform obj)
    {
        StartCoroutine(StartKnockback(knockbackPwrX, knockbackPwrY, obj));
    }
    public IEnumerator StartKnockback(float knockbackPwrX, float knockbackPwrY, Transform obj){

        float knockDur = 0.2f;
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

    public IEnumerator ShowHealth(){
        healthBarTime++;
        healthBarVisual.SetActive(true);
        if(healthBarTime > 0)
            yield return new WaitForSeconds(4);
            healthBarTime--;
        if(healthBarTime == 0)
            healthBarVisual.SetActive(false);
    }


    public void Damage(int damage) {
        GetComponent<AudioSource>().Play();
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        StartCoroutine(ShowHealth());

        if(currentHealth <= 0) {
            for(int i = 0; i < coinAmount; i++)
                Instantiate(coinDrop, transform.position, Quaternion.identity);
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
                sprite.localScale = new Vector3(-1f * Mathf.Abs(sprite.localScale.x), sprite.localScale.y, sprite.localScale.z);
            }
            else if (rb.velocity.x < -0.05f)
            {
                sprite.localScale = new Vector3(Mathf.Abs(sprite.localScale.x), sprite.localScale.y, sprite.localScale.z);
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
