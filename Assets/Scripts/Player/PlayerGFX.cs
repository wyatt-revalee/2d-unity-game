using System;
using UnityEngine;

public class PlayerGFX : MonoBehaviour
{
    private Transform playerSprite;
    private Rigidbody2D rigidbody2d;
    public Animator animator;
    public Player player;
    public PlayerMovement playerMovement;

    // Movement
    private bool isCrouching;
    private bool isJumping;
    private bool isFalling;
    public int direction;
    
    private Vector3 left;
    private Vector3 right;

    // Start is called before the first frame update
    void Start()
    {
        playerSprite = GetComponent<Transform>();
        right = playerSprite.localScale;
        left = new Vector3 (playerSprite.localScale.x * -1, playerSprite.localScale.y, playerSprite.localScale.z);
        rigidbody2d = transform.GetComponent<Rigidbody2D>();
        direction = 1;

    }

    // Update is called once per frame
    void Update() {
        
        HandleMovementAnimations();

        animator.SetFloat("Speed", Math.Abs(rigidbody2d.velocity.x));
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsCrouching", isCrouching);
        animator.SetBool("IsGrounded", playerMovement.IsGrounded());
        animator.SetBool("IsFalling", isFalling);

        SetCharDirection();
        
    }

    private void HandleMovementAnimations() {

        // Jumping and falling
        if (!playerMovement.IsGrounded() && rigidbody2d.velocity.y >= 0) {
            isJumping = true;
        }
        else if(!playerMovement.IsGrounded() && rigidbody2d.velocity.y < 0) {
            isJumping = false;
            isFalling = true;
        } else {
            isJumping = false;
            isFalling = false;
        }

            // Crouch
        if (playerMovement.IsCrouching()) {
            isCrouching = true;
        } else {
            isCrouching = false;
        }
    }

    private void SetCharDirection() {

        if(playerMovement.playerCanMove == false)
            return;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            direction = -1;
        else if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            direction = 1;


        bool flipX = (direction < 0.0f)
        ? true
        : false;

        if(flipX)
            playerSprite.localScale = left;
        else
            playerSprite.localScale = right;
    }
}
