using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections;
 
public class Player : MonoBehaviour, IDamageable{


    // Rendering
    [SerializeField] private LayerMask platformsLayerMask;
    public Component[] colliders;
    public Collider2D physicsCollider;
    public PlayerMovement playerMovement;
    public Animator animator;
    public Animator sceneTransition;
    private GameObject playerSpawnPoint;
    private int currentScene;
    private bool playerMoved;
    public bool isGrounded;
    private int currentWorld = 0;

    // Health and Inventory
    [Header("Health and Combat")]
    public CoinCounter coinCounter;
    public HealthBar healthBar;
    public ManaBar manaBar;
    public LayerMask enemyLayers;
    public Transform attackPoint;
    public AxeProjectileBehavior secondaryProjectile;
    public Transform LaunchOffset;
    public GameObject spinCooldownIcon;
    private AbilityIcon spinAttackScript;
    public int maxHealth = 10;
    public int currentHealth;
    public float maxMana = 100f;
    public float currentMana;
    public int attackDamage;
    public int baseDamage = 2;
    public int meleeDamage;
    public int rangedDamage;
    public int heavyDamage;
    public float attackRange = 0.3f;
    public float knockbackY = 15;
    public float knockbackX = 15;
    public float attackSpeed = 2f;
    private float manaRegenSpeed = 0.2f;
    float nextAttackTime = 0f;
    private float nextSpinTime = 0f;
    private float spinCooldown = 5f;

    [Header("Inventory")]
    public InventoryItemData coin;
    public InventorySystem inventory;
    public int coinCount;


    [Header("Iframe Variables")]
    public Collider2D combatCollider;
    public SpriteRenderer playerSprite;
    public Color flashColor;
    public Color regularColor;
    public float flashDuration;
    public int numberOfFlashes;

    public ManageLevels managerScript;


    // Use this for initialization
    private void Start () {

        meleeDamage = baseDamage;
        rangedDamage = baseDamage-1;
        heavyDamage = baseDamage+1;
        attackDamage = meleeDamage;
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(currentHealth);
        currentMana = maxMana;
        manaBar.SetMaxMana(maxMana);
        playerSprite.color = managerScript.worldColors[managerScript.world];
        spinAttackScript = spinCooldownIcon.GetComponent<AbilityIcon>();

    }
 
    // Update is called once per frame
    private void Update ()
     {

        if(managerScript.world > currentWorld)
        {
            currentWorld = managerScript.world;
            playerSprite.color = managerScript.worldColors[managerScript.world];
        }

        isGrounded = playerMovement.IsGrounded();

        if(playerMovement.playerCanMove == true)
        {
            
            if(Time.time >= nextAttackTime)
            {
                if(Input.GetKeyDown(KeyCode.F) && Time.time >= nextSpinTime)
                {
                    spinAttackScript.StartCooldown();
                    StartCoroutine(SpinAttack());
                    nextAttackTime = Time.time + 1f / attackSpeed;
                    nextSpinTime = Time.time + spinCooldown;
                }
                if(Input.GetMouseButtonDown(0))
                {
                    StartCoroutine(PrimaryAttack());
                    nextAttackTime = Time.time + 1f / attackSpeed;
                }
                if(!isGrounded && Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.S))
                {
                    StartCoroutine(AtgSlam());
                    nextAttackTime = Time.time + 1f / attackSpeed;
                }
                
                else if(Input.GetMouseButtonDown(1))
                {
                    SecondaryAttack();
                    nextAttackTime = Time.time + 1f / attackSpeed;
                }
                
            }
        }

        if(sceneTransition == null)
            sceneTransition = GameObject.Find("SceneLoader").transform.GetChild(0).GetComponent<Animator>();
        
    }

    private void FixedUpdate() 
    {
        if(playerMovement.isPaused == false)
        {
            if(currentMana < maxMana)
            {
                currentMana += manaRegenSpeed;
                // if(currentMana > maxMana)
                //     currentMana = maxMana;
                manaBar.SetMana(currentMana);
            }
        }

    }


    public void Damage(int damage) {

        StartCoroutine(FlashCo());

        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if(currentHealth <= 0) {
            PlayerDeath();
        }

    }

    public void Heal() {
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);
    }

    public void Purchase(int coinCost) {
        coinCount -= coinCost;
        inventory.m_itemDictionary["coin"].stackSize -= coinCost;
        coinCounter.SetCoinCount(coinCount);
    }

    IEnumerator PrimaryAttack()
    {
        //Play animation
        attackDamage = meleeDamage;
        animator.SetTrigger("Attack");
        animator.SetBool("IsAttacking", true);
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("IsAttacking", false);

    }

    void SecondaryAttack()
    {
        float manaCost = 20f;
        if(currentMana >= manaCost)
        {
            attackDamage = rangedDamage;
            //Play animation
            animator.SetTrigger("Attack");
            //Create Projectile
            var projectile = Instantiate(secondaryProjectile, LaunchOffset.position, transform.rotation);
            manaBar.SetMana(currentMana -= manaCost);
        }
    }

    IEnumerator AtgSlam()
    {
        attackDamage = heavyDamage;
        animator.SetTrigger("ATG");
        animator.SetBool("IsAttacking", true);
        yield return new WaitUntil(() => (isGrounded) == true);
        animator.SetBool("IsAttacking", false);
    }

    IEnumerator SpinAttack()
    {
        attackDamage = meleeDamage;
        animator.SetTrigger("SpinAttack");
        animator.SetBool("IsAttacking", true);
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("IsAttacking", false);
    }

    void PlayerDeath()
    {
        StartCoroutine(GameOver());
    }

    private IEnumerator FlashCo()
    {
        int temp = 0;
        combatCollider.enabled = false;
        while(temp < numberOfFlashes)
        {
            playerSprite.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            playerSprite.color = regularColor;
            yield return new WaitForSeconds(flashDuration);
            temp++;
        }
        playerSprite.color = managerScript.worldColors[managerScript.world];
        combatCollider.enabled = true;
    }

    IEnumerator GameOver()
    {
        Debug.Log("GAME OVER");
        sceneTransition.SetTrigger("End");

        yield return new WaitForSeconds(0.8f);
        SceneManager.LoadScene(1);
    }
}