using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections;
 
public class PlayerMovement : MonoBehaviour {


    // Rendering
    [SerializeField] private LayerMask platformsLayerMask;
    private Rigidbody2D rigidbody2d;
    private bool isCrouching;
    public Collider2D physicsCollider;

    // public bool playerCanMove;

    public Player playerHasControl;



    // Use this for initialization
    private void Start () {

        // playerCanMove = true;

        playerHasControl = GetComponent<Player>();

        rigidbody2d = transform.GetComponent<Rigidbody2D>();

    }
 
    // Update is called once per frame
    private void Update () {

        Debug.Log(playerCanMove);
        HandleMovement();

    }


    private void HandleMovement() {
        float moveSpeed = 5f;

        // if(playerCanMove == false)
        //     return;

        

        // Jump
        if (IsGrounded() && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))) {
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
        if(Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
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
    

    public IEnumerator Knockback(float knockDur, float knockbackPwrX, float knockbackPwrY, Transform obj){

    //  playerCanMove = false;
     float timer = 0;
 
     while( knockDur > timer ) {
         timer += Time.deltaTime;
         Vector2 direction = (obj.transform.position - this.transform.position).normalized;
         direction.x *= knockbackPwrX;
         direction.y *= knockbackPwrY;
         rigidbody2d.AddForce(-direction);
     }

 
     yield return 0; 
 
    //  playerCanMove = true;
 }
}