using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
 
public class PlayerMovement : MonoBehaviour, IKnockbackable {


    // Rendering
    [SerializeField] private LayerMask platformsLayerMask;
    private Rigidbody2D rigidbody2d;
    private bool isCrouching;
    public Collider2D physicsCollider;


    //Gameflow
    public GameObject pauseMenu;
    public bool isPaused;
    public bool playerCanMove;

    //pauseMenu.transform.GetChild(0).gameObject;


    // Use this for initialization
    private void Start () {

        playerCanMove = true;

        rigidbody2d = transform.GetComponent<Rigidbody2D>();

    }
 
    // Update is called once per frame
    private void Update () {

        HandleMovement();

    }


    private void HandleMovement() {
        float moveSpeed = 5f;

        if(Input.GetKeyDown(KeyCode.Escape))
            PauseControl();

        if(playerCanMove == false)
            return;

        

        // Jump
        if (IsGrounded() && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKey(KeyCode.Space))) {
            float jumpVelocity = 7f;
            rigidbody2d.velocity = Vector2.up * jumpVelocity;
        }



        // Left and Right movement
        if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) && !IsCrouching()) {
            rigidbody2d.velocity = new Vector2(-moveSpeed, rigidbody2d.velocity.y);
        } else {
            if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) && !IsCrouching()) {
            rigidbody2d.velocity = new Vector2(+moveSpeed, rigidbody2d.velocity.y);
            } else {
            // No keys pressed
            rigidbody2d.velocity = new Vector2(0, rigidbody2d.velocity.y);
            }
        }
    }

    public bool IsCrouching() {
        if((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && playerCanMove == true)
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
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
            playerCanMove = false;
            return;
        }

        if(isPaused == true)
        {
            pauseMenu.transform.GetChild(1).gameObject.SetActive(true);
            pauseMenu.transform.GetChild(2).gameObject.SetActive(false);
            pauseMenu.transform.GetChild(3).gameObject.SetActive(false);
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
            playerCanMove = true;
            return;
        }

    }
}