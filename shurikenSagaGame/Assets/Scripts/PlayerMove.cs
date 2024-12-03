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
    public bool isShoot = false;
    

    //Sprites
    public Transform player;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    public Sprite shuriSprite;

    //Projectiles
    public Transform firePoint;
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public float attackRate = 2f;
    private float nextAttackTime = 0f;
    
    //Sounds
    public AudioSource footstepSFX;
    private bool isMoving = false; // Track movement state

    // dashing vars (Massimo)
    [SerializeField]
    public float dashSpeed = 25f; // Speed during dash
    [SerializeField]
    public float dashDuration = .5f; // Duration of the dash
    [SerializeField]
    public float dashCooldown = .5f; // Cooldown time between dashes
    [SerializeField]
    public float dashLength = 10f; // Distance the player dashes

    public float doubleTapTime = 0.22f; // Time window for double-tap detection
    private float lastTapTime = 0f; // Last time a movement key was tapped
    private string lastTappedKey = ""; // Track the last tapped movement key

    private bool isDashing = false;

    public float timePassed = 0;
    public bool isVisible = true;

    private Vector3 lastSavedPosition;
    private bool fell = false;

    private GameHandler gh;
    public AudioSource shuriThrow;

    [SerializeField]
    public float kbForce;
    public static bool dashUnlocked;
    public static bool shurikenUnlocked;
    //private bool knockedBack = false;
    private Vector2 knockbackDirection;
    private float knockbackTime = 0f; // Time remaining for knockback
    private float knockbackDecayRate = 5f; // Rate of knockback decay

    void Start(){
        gh = GameObject.Find("GameHandler").GetComponent<GameHandler>();
        lastSavedPosition = transform.position;

        shuriThrow.enabled = true;

        // Get the SpriteRenderer component from the player_art child
        spriteRenderer = transform.Find("player_art").GetComponent<SpriteRenderer>();

        // Get the Animator component from the player_art child
        animator = transform.Find("player_art").GetComponent<Animator>();

        InvokeRepeating("SavePosition", 1, 1);
    }

    private void SavePosition()
    {
        if (!fell)
        {
            lastSavedPosition = transform.position;
        }
    }

    void Update(){
        if (!isAlive) return;

        //NOTE: Horizontal axis: [a] / left arrow is -1, [d] / right arrow is 1
        //NOTE: Vertical axis: [w] / up arrow, [s] / down arrow
        Vector3 hvMove = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);
        Vector2 movement = hvMove * runSpeed * Time.deltaTime;
        rb2D.velocity = movement;

        isMoving = hvMove != Vector3.zero;
        bool MovingHoriz = hvMove.x != 0;  // Check if horizontal movement is happening

        animator.SetFloat("Vertical", hvMove.y);  // Send the vertical movement to the Animator
        animator.SetBool("MovingHoriz", MovingHoriz);  // Send the horizontal movement != checker to the Animator
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsDashing", isDashing);
        animator.SetBool("IsShooting", isShoot);

        if (isMoving && !footstepSFX.isPlaying) {
            footstepSFX.Play();
            //Debug.Log("Playing Footstep sound..");
        } else if (!isMoving && footstepSFX.isPlaying) {
            footstepSFX.Stop();
            //Debug.Log("Stopping Footstep sound..");
        }

        // Turning. Reverse if input is moving the Player right and Player faces left.
        if ((hvMove.x < 0 && !FaceLeft) || (hvMove.x > 0 && FaceLeft)){
            playerTurn();
        }

        if (isShoot) {
            animator.enabled = false;
            spriteRenderer.sprite = shuriSprite; //Hide non- moving
        } else {
            animator.enabled = true; //Enable animator
        }

        if (Time.time >= nextAttackTime){
            if (Input.GetAxis("AttackYea") > 0 && shurikenUnlocked){
                playerFire();
                nextAttackTime = Time.time + 1f / attackRate;
                isShoot = true;
            } else {
                isShoot = false;
            }
        }

        // Massimo changes start*******
        // Check for double-tap on movement keys
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        if (moveVertical > 0 && Input.GetKeyDown(KeyCode.W))
        {
            HandleDoubleTap("W");
        }
        else if (moveVertical < 0 && Input.GetKeyDown(KeyCode.S))
        {
            HandleDoubleTap("S");
        }
        else if (moveHorizontal < 0 && Input.GetKeyDown(KeyCode.A))
        {
            HandleDoubleTap("A");
        }
        else if (moveHorizontal > 0 && Input.GetKeyDown(KeyCode.D))
        {
            HandleDoubleTap("D");
        }
        //Massimo changes end*******
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Hole") && !isDashing)
        {
            fell = true;
            StartCoroutine(Wait());
            gh.playerHealth -= 20;
        }
        if (collision.CompareTag("enemy"))
        {
            if (!gh.isImmune)
            {
                gh.hit = true;
                //rb2D.AddForce(knockbackDirection * kbForce, ForceMode2D.Impulse);
                knockbackDirection = (transform.position - collision.transform.position).normalized * kbForce;
                Debug.Log("Amount of force: " + knockbackDirection);
                //knockedBack = true;
                knockbackTime = 1f;
                gh.playerHealth -= collision.gameObject.GetComponent<BasicEnemyValues>().damageAmount;
            }
        }
        else if (collision.CompareTag("noswitch"))
        {
            gh.noSwitchZone = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.CompareTag("noswitch"))
        //{
        //    gh.noSwitchZone = true;
        //}
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("noswitch"))
        {
            gh.noSwitchZone = false;
        }
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(2); // Wait for 2 seconds
        Debug.Log("2 seconds have passed!");
        Respawn();
        // Add any actions you want to perform after the wait here
    }

    public void Respawn()
    {
        fell = false;
        transform.position = lastSavedPosition;
    }

    private void FixedUpdate()
    { 
        if (isAlive && !isDashing && !fell)
        {
            Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * runSpeed * Time.fixedDeltaTime * 33;
            if (knockbackTime > 0)
            {
                rb2D.velocity = movement + knockbackDirection;

                // Gradually reduce knockback effect
                knockbackDirection = Vector2.Lerp(knockbackDirection, Vector2.zero, Time.fixedDeltaTime * knockbackDecayRate);
                knockbackTime -= Time.fixedDeltaTime;
            }
            else
            {
                rb2D.velocity = movement; // Normal movement
                knockbackDirection = Vector2.zero; // Reset knockback
            }
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
        } else if ( isAlive && isDashing && !fell)
        {
            //Debug.Log(Time.fixedDeltaTime);
            if (Time.time > timePassed + 0.037)
            {
                timePassed = Time.time;
                ToggleVis();
            }
        } else if (fell)
        {
            Vector2 movement = new Vector2(0, 0);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
        }
    }

    private void ToggleVis() { 
        if(isVisible)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, .6f);
        } else
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, .8f);
        }
        isVisible = !isVisible;

    }

    //Massimo changes start*******
    private void HandleDoubleTap(string key)
    {
        if (key == lastTappedKey && Time.time - lastTapTime <= doubleTapTime && dashUnlocked)
        {
            // If the same key is tapped twice within the time window, dash
            StartCoroutine(Dash(key));
        }

        // Update last tapped key and time
        lastTappedKey = key;
        lastTapTime = Time.time;
    }

    IEnumerator Dash(string directionKey)
    {
        isDashing = true;

        // Determine the dash direction based on the key
        Vector2 direction = Vector2.zero;
        if (directionKey == "W")
        {
            direction = Vector2.up;

        }
        else if (directionKey == "S")
        {
            direction = Vector2.down;
        }
        else if (directionKey == "A") 
        {
            direction = Vector2.left;
        }
        else if (directionKey == "D")
        {
            direction = Vector2.right;
        }

        float elapsedTime = 0f;

        // Store the initial position to calculate the distance
        Vector2 startPosition = rb2D.position;
        while (elapsedTime < dashDuration)
        {
            // Calculate the new position based on dash length and speed
            Vector2 dashPosition = startPosition + direction * dashLength;

            // Move to the dash position instantly
            rb2D.MovePosition(Vector2.MoveTowards(rb2D.position, dashPosition, dashSpeed * Time.fixedDeltaTime / 10));

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Stop the dash
        rb2D.velocity = Vector2.zero;

        // Allow for immediate resumption of regular movement
        isDashing = false;
    }
    //Massimo changes end*******

    private void playerTurn(){
        // NOTE: Switch player facing label
        FaceLeft = !FaceLeft;

        // NOTE: Multiply player's x local scale by -1.
        Vector3 theScale = spriteRenderer.transform.localScale;
        theScale.x *= -1;
        spriteRenderer.transform.localScale = theScale;
    }

    void playerFire() {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // Set z to 0 since we're working in 2D

        // Calculate the direction from the firing point to the mouse position
        Vector2 fwd = (mousePosition - firePoint.position).normalized;

        // Play the shuriken throw sound
        if (shuriThrow != null) {
            if (!shuriThrow.isPlaying) { 
                //shuriThrow.Play(); 
            }
        }

        // Instantiate the projectile and apply force towards the mouse position
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.GetComponent<Rigidbody2D>().AddForce(fwd * projectileSpeed, ForceMode2D.Impulse);

        // Set the sprite to the default sprite
        spriteRenderer.sprite = shuriSprite; // Show non-moving default sprite
    }

}