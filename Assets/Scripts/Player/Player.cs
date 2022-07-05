using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections;
 
public class Player : MonoBehaviour {


    // Rendering
    [SerializeField] private LayerMask platformsLayerMask;
    public Component[] colliders;
    public Collider2D physicsCollider;
    public Animator animator;

    // Health and Lives
    [Header("Health and Combat")]
    public LifeCounter lifeCounter;
    public HealthBar healthBar;
    public LayerMask enemyLayers;
    public Transform attackPoint;
    public int lifeCount = 3;
    public int maxHealth = 10;
    public int currentHealth;
    public int attackDamage = 1;
    public float attackRange = 0.3f;
    public float knockback = 5;
    // public int attackSpeed;

    [Header("Iframe Variables")]
    public Collider2D combatCollider;
    public SpriteRenderer playerSprite;
    public Color flashColor;
    public Color regularColor;
    public float flashDuration;
    public int numberOfFlashes;

    // Use this for initialization
    private void Start () {
        
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        lifeCounter.SetLives(lifeCount);
 
    }
 
    // Update is called once per frame
    private void Update () {
 
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            Attack();
        


    }


    public void TakeDamage(int damage) {

        StartCoroutine(FlashCo());

        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if(currentHealth == 0) {
            lifeCount -= 1;
            lifeCounter.SetLives(lifeCount);
            healthBar.SetHealth(maxHealth);
            currentHealth = maxHealth;
        }

        if(lifeCount == 0) {
            Destroy(gameObject);
            SceneManager.LoadScene(2);

        }

    }

    private void OnDrawGizmosSelected()
    {
        if(attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    void Attack()
    {

        //Play animation
        animator.SetTrigger("Attack");

        // Deteck enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Damage them and add knockback
        foreach(Collider2D enemyCollider in hitEnemies)
        {
            Rigidbody2D enemy = enemyCollider.GetComponent<Rigidbody2D>();
            Vector2 difference = enemy.transform.position - transform.position;
            difference.y += 2;
            difference = difference.normalized * knockback;
            enemy.AddForce(difference, ForceMode2D.Impulse);
            // enemy.GetComponent<SlimeAI>().TakeDamage(attackDamage);
        }
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
        // playerCanMove = true;
    }
}