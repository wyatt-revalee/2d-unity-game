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
    public float manaRegenSpeed = 12f;
    float nextAttackTime = 0f;

    [Header("Iframe Variables")]
    public Collider2D combatCollider;
    public SpriteRenderer playerSprite;
    public Color flashColor;
    public Color regularColor;
    public float flashDuration;
    public int numberOfFlashes;


    // Use this for initialization
    private void Start () {

        sceneTransition = GameObject.Find("SceneLoader").transform.GetChild(0).GetComponent<Animator>();

        currentScene = SceneManager.GetActiveScene().buildIndex;

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        currentMana = maxMana;
        manaBar.SetMaxMana(maxMana);
 
    }
 
    // Update is called once per frame
    private void Update ()
     {

        if(SceneManager.GetActiveScene().buildIndex > currentScene)
        {
            playerMoved = false;
            playerSpawnPoint = GameObject.Find("PlayerSpawnPoint");
            currentScene += 1;
            transform.position = playerSpawnPoint.transform.position;
            sceneTransition = GameObject.Find("SceneLoader").transform.GetChild(0).GetComponent<Animator>();

        }
        if(SceneManager.GetActiveScene().buildIndex == 2 && playerMoved == false)
        {
            playerSpawnPoint = GameObject.Find("PlayerSpawnPoint");
            transform.position = playerSpawnPoint.transform.position;
            playerMoved = true;
            sceneTransition = GameObject.Find("SceneLoader").transform.GetChild(0).GetComponent<Animator>();

        }

        if(playerMovement.isPaused == false)
        {
            if(currentMana < maxMana)
            {
                currentMana += 0.015f;
                // if(currentMana > maxMana)
                //     currentMana = maxMana;
                manaBar.SetMana(currentMana);
            }
            
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


        
    }


    public void Damage(int damage) {

        StartCoroutine(FlashCo());

        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if(currentHealth <= 0) {
            PlayerDeath();
        }

    }

    public void Heal(int coinCost) {

        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);

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
        combatCollider.enabled = true;
    }

    IEnumerator GameOver()
    {
        Debug.Log("GAME OVER");
        sceneTransition.SetTrigger("Start");

        yield return new WaitForSeconds(0.8f);
        SceneManager.LoadScene(1);
    }
}