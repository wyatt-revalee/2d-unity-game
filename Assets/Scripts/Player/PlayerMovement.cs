using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
 
public class PlayerMovement : MonoBehaviour, IKnockbackable {


    // Rendering
    [SerializeField] private LayerMask platformsLayerMask;
    public Rigidbody2D rigidbody2d;
    private bool isCrouching;
    public Collider2D physicsCollider;
    private static GameObject Instance;


    //Gameflow
    public GameObject pauseMenu;
    public GameObject inventory;
    public bool isPaused;
    public bool playerCanMove;
    public bool inventoryIsOpen;



    // Use this for initialization
    private void Start () {

        pauseMenu = GameObject.Find("PauseMenu");
        // inventory = GameObject.Find("Inventory");

        isPaused = false;

        if(Instance == null)
        {
            Instance = gameObject;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }

        playerCanMove = true;
        rigidbody2d = transform.GetComponent<Rigidbody2D>();

    }
 
    // Update is called once per frame
    private void Update () {

        if(SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1)
            Destroy(gameObject);
        HandleMovement();

    }


    private void HandleMovement() {
        float moveSpeed = 5f;

        if(Input.GetKeyDown(KeyCode.Escape))
            PauseControl();

        if(isPaused == true)
            return;

        if(Input.GetKeyDown(KeyCode.Tab))
            InventoryControl();

        if(playerCanMove == false)
            return;

        

        // Jump
        if (IsGrounded() && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKey(KeyCode.Space))) {
            float jumpVelocity = 7f;
            rigidbody2d.velocity = Vector2.up * jumpVelocity;
        }



        // Left and Right movement
        if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) && !IsCrouching() && !(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))) {
                rigidbody2d.velocity = new Vector2(-moveSpeed, rigidbody2d.velocity.y);
                return;
        } else {
            if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) && !IsCrouching() && !(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))) {
                rigidbody2d.velocity = new Vector2(+moveSpeed, rigidbody2d.velocity.y);
                return;
            } else {
            // No keys pressed
            rigidbody2d.velocity = new Vector2(0, rigidbody2d.velocity.y);
            }
        }
    }

    public bool CheckMovement()
    {
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            return true;
        else
            return false;
    }

    public bool IsCrouching() {
        if((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && playerCanMove == true && IsGrounded() && !CheckMovement())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public bool IsGrounded() {
        RaycastHit2D raycastHit2d = Physics2D.BoxCast(physicsCollider.bounds.center, physicsCollider.bounds.size, 0f, Vector2.down, .1f, platformsLayerMask);
        return raycastHit2d.collider != null;
    }
    

    public void Knockback(float knockbackPwrX, float knockbackPwrY, Transform obj)
    {
        StartCoroutine(StartKnockback(knockbackPwrX, knockbackPwrY, obj));
    }

    public IEnumerator StartKnockback(float knockbackPwrX, float knockbackPwrY, Transform obj){

        float knockDur = 0.5f;
        float timer = 0;
        
        playerCanMove = false;

        while( knockDur > timer ) {
            timer += Time.deltaTime;
            Vector2 direction = (obj.transform.position - this.transform.position).normalized;
            direction.y = -1;
            direction.x *= knockbackPwrX;
            direction.y *= knockbackPwrY;
            rigidbody2d.AddForce(-direction);
        }

    
        yield return new WaitForSeconds(0.5f); 
        playerCanMove = true;
    }

    public void PauseControl()
    {
        if(isPaused == false)
        {
            Time.timeScale = 0f;
            isPaused = true;
            playerCanMove = false;
            Debug.Log("Paused");
            pauseMenu.SetActive(true);
            return;
        }

        if(isPaused == true)
        {
            pauseMenu.transform.GetChild(1).gameObject.SetActive(true);
            pauseMenu.transform.GetChild(2).gameObject.SetActive(false);
            pauseMenu.transform.GetChild(3).gameObject.SetActive(false);
            pauseMenu.SetActive(false);
            if(inventoryIsOpen == false)
            {
                Time.timeScale = 1f;
                playerCanMove = true;
            }
            isPaused = false;
            return;
        }

    }

    public void InventoryControl()
    {
        if(inventoryIsOpen == false)
        {
            Time.timeScale = 0f;
            inventoryIsOpen = true;
            playerCanMove = false;
            Debug.Log("Inventory Opened");
            inventory.SetActive(true);
            return;
        }

        if(inventoryIsOpen == true)
        {
            inventory.SetActive(false);
            Time.timeScale = 1f;
            inventoryIsOpen = false;
            playerCanMove = true;
            return;
        }

    }
}