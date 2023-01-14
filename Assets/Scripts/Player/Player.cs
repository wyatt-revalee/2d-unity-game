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

    // Health and Money
    [Header("Health and Combat")]
    public CoinCounter coinCounter;
    public HealthBar healthBar;
    public ManaBar manaBar;
    public LayerMask enemyLayers;
    public Transform attackPoint;
    public AxeProjectileBehavior secondaryProjectile;
    public Transform LaunchOffset;
    public int coins;
    public int maxHealth = 10;
    public int currentHealth;
    public float maxMana = 100f;
    public float currentMana;
    public int attackDamage = 1;
    public float attackRange = 0.3f;
    public float knockbackY = 15;
    public float knockbackX = 15;
    public float attackSpeed = 2f;
    private float manaRegenSpeed = 0.2f;
    float nextAttackTime = 0f;

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

    //    if (SceneManager.GetActiveScene().buildIndex != 3)
    //         SceneManager.LoadScene(3);

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        currentMana = maxMana;
        manaBar.SetMaxMana(maxMana);
        playerSprite.color = managerScript.worldColors[managerScript.world];

    }
 
    // Update is called once per frame
    private void Update ()
     {

        isGrounded = playerMovement.IsGrounded();

        if(playerMovement.isPaused == false)
        {
            
            if(Time.time >= nextAttackTime)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    PrimaryAttack();
                    nextAttackTime = Time.time + 1f / attackSpeed;
                }
                if(Input.GetMouseButtonDown(1))
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
        coins -= coinCost;
        coinCounter.SetCoinCount(coins);
    }

    void PrimaryAttack()
    {
        //Play animation
        animator.SetTrigger("Attack");
        
    }

    void SecondaryAttack()
    {
        float manaCost = 20f;
        if(currentMana >= manaCost)
        {
            //Play animation
            animator.SetTrigger("Attack");
            //Create Projectile
            Instantiate(secondaryProjectile, LaunchOffset.position, transform.rotation);
            manaBar.SetMana(currentMana -= manaCost);
        }
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