
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour {


    //public AudioSource WalkSFX;
    public Rigidbody2D rb2D;
    private bool FaceLeft = false; // determine which way player is facing.
    public static float runSpeed = 10f;
    public float startSpeed = 10f;
    public bool isAlive = true;

    // Reference to the SpriteRenderer component in Player_Art
    private SpriteRenderer spriteRenderer;

    public Transform player;

    // Reference to the Animator component in Player_Art
    //private Animator animator;

    public Sprite defaultSprite;
    public Sprite sideSprite;
    public Sprite backSprite;

    void Start(){
        rb2D = transform.GetComponent<Rigidbody2D>();

        // Get the SpriteRenderer component from the player_art child
        spriteRenderer = transform.Find("player_art").GetComponent<SpriteRenderer>();

        // Get the Animator component from the player_art child
        //animator = transform.Find("player_art").GetComponent<Animator>();
    }

    void Update(){
        //NOTE: Horizontal axis: [a] / left arrow is -1, [d] / right arrow is 1
        //NOTE: Vertical axis: [w] / up arrow, [s] / down arrow
        Vector3 hvMove = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);
        if (isAlive == true){
            //transform.position = transform.position + hvMove * runSpeed * Time.deltaTime;
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector2 movement = new Vector2(moveHorizontal, moveVertical) * runSpeed;
            rb2D.velocity = movement;

            if (hvMove.y < 0) {
                //animator.enabled = true; // Enable Animator for front view
                spriteRenderer.sprite = defaultSprite; //Show non-moving default sprite
            } else if (hvMove.y > 0) {
                //animator.enabled = true; // Enable Animator for back view
                spriteRenderer.sprite = backSprite; //Show non-moving default sprite
            } else if (hvMove.x != 0) {
                //animator.enabled = true; // Enable Animator for side view
                spriteRenderer.sprite = sideSprite; //Show non-moving default sprite
            } else {
                //animator.enabled = false; // Disable Animator
                spriteRenderer.sprite = defaultSprite; //Show non-moving default sprite
            }
            // Turning. Reverse if input is moving the Player right and Player faces left.
            if ((hvMove.x < 0 && !FaceLeft) || (hvMove.x > 0 && FaceLeft)){
                playerTurn();
            }
        }
    }

    private void playerTurn(){
        // NOTE: Switch player facing label
        FaceLeft = !FaceLeft;

        // NOTE: Multiply player's x local scale by -1.
        Vector3 theScale = spriteRenderer.transform.localScale;
        theScale.x *= -1;
        spriteRenderer.transform.localScale = theScale;
    }
}