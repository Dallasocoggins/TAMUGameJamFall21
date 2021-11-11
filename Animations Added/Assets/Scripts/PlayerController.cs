using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // Max horizontal speed
    public float speed;

    // How high you jump
    public float jumpHeight;

    // How many times you can jump past the first jump
    // Ex: 1 for double jump, 2 for triple jump, 0 for normal
    public int extraJumps;

    // How fast you jump away from walls
    public float horizontalJumpSpeed;

    // true if you want to stick to walls
    public bool wallClimb;

    // How long we ignore user horizontal input and wall stickyness
    public float wallJumpCooldown;

    // The speed lmit for falling down walls
    // Needs to be positive
    public float wallSlideSpeed;

    // How long (time) we want our dash to last
    public float dashTimeLength;

    // How fast we move while in a dash
    public float dashSpeed;

    // How many dashes the user can do after leaving the ground
    public int numDashes;

    // When we do a box cast to determine if we are next to a wall, how far we shave off the top so we don't mistake the moving platform we are standing on for a wall
    // Make this number bigger if there are fast moving vertical platforms in the level
    public float verticalMargin;


    // The disatnce at which wallclimb becomes active
    // Make this larger if there are fast moving platforms
    public float wallClimbMargin;

    // The distance at which the player is considered grounded
    public float groundMargin;

    public Animator animator;

    [SerializeField]
    LayerMask platformLayerMask;

    // true if the character is facing right
    private bool facingRight;

    // We use this for flipping only the graphics of our charater without changing our hitbox or anything else
    private Transform playerVisual;

    private SpriteRenderer spriteRenderer;
    new private Collider2D collider;
    private Rigidbody2D rb;

    // Horizintal velocity
    private float movement;
    private float movementInput;

    // true when the user presses jump and we need to make the character jump
    private bool jumped;

    // Keeps track of how many more times the user can jump
    private int jumpsLeft;

    // Keeps track of a "cooldown" period, where we ignore the user's horizontal input and wall-stickyness
    private float timeAfterWallJump;

    // When we want the player to stay vertically still, we set gravity to 0. This is so we can turn it back on
    private float gravity;

    // true when the user presses dash and we need to make the charater dash
    private bool dashed;

    // How much longer our dash is going to last
    private float timeAfterDash;

    // How many dashes we have left
    private int dashesLeft;

    private CharacterController controller;

    // Not sure if this should be public or private
    // Where you go when you respawn
    public Vector3 checkpoint;

    [SerializeField]
    private bool isPlayerOne;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        playerVisual = transform.GetChild(0);
        facingRight = true;
        gravity = rb.gravityScale;
        checkpoint = transform.position;

        controller = gameObject.GetComponent<CharacterController>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<float>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // If the user is allowed to jump
        if (!jumped && (jumpsLeft != 0 || IsGrounded()))
        {
            jumped = context.action.triggered;
        }
    }

    public void OnPower(InputAction.CallbackContext context)
    {
        UsePower();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        // Get input for dash
        if (!dashed && timeAfterDash <= 0 && dashesLeft > 0)
        {
            dashed = context.action.triggered;
        }
    }

    // Update is called once per frame
    // Input collection and animation
    void Update()
    {
        #region input

        // if we are doing a special ability, we don't want to reset movement just yet
        if (timeAfterWallJump <= 0)
        {
            // When dashing, the ability "fades away" to normal movement.
            float dashMovement = dashSpeed;
            if (!facingRight)
                dashMovement *= -1;
            movement = movementInput * speed * (dashTimeLength - timeAfterDash) / dashTimeLength + dashMovement * timeAfterDash / dashTimeLength;

        }

        /*
        // This will turn off wall climb if the user is holding down
        if (Input.GetAxis("Vertical") < 0)
        {
            wallClimb = false;
        }
        else
        {
            wallClimb = true;
        }
        */

        // To reset jumpsLeft
        if (IsGrounded())
        {
            jumpsLeft = extraJumps;
            dashesLeft = numDashes;
        }

        #endregion


        #region animation

        animator.SetFloat("Hrzntal_Speed", Mathf.Abs(movement));
        animator.SetBool("IsJumping", jumped);

        if ((movement < 0 && facingRight) || (movement > 0 && !facingRight))
            transform.Rotate(0f, 180f, 0f);

        // Updating facingRight
        if (movement > 0)
            facingRight = true;
        if (movement < 0)
            facingRight = false;

        #endregion

        // I was having a wierd issue where if you hold down move while next to a wall, you don't fall
        // This seems to fix that
        if (NextToWall() && timeAfterWallJump <= 0)
        {
            if (NextToLeftWall() && movement < 0 || NextToRightWall() && movement > 0)
            {
                movement = 0;
            }
        }

        // If we are on top of a moving platform or clinging to one but not dashing, we should change movement so that we "stick" to the surface horizontally
        // Put this part last so the way you are facing is not affected by this
        if (timeAfterWallJump <= 0 && (OnMovingPlatform() || (MovingPlatformWall() && wallClimb)) && timeAfterDash <= 0)
        {
            movement += ((MovingPlatformController)(MovingPlatform().GetComponentInParent(typeof(MovingPlatformController)))).Velocity().x;
        }
    }

    // Physics calculations
    private void FixedUpdate()
    {
        // Make the wall jump cooldown decrease
        if (timeAfterWallJump > 0)
        {
            timeAfterWallJump -= Time.deltaTime;
        }

        // Make the dash cooldown decrease
        if (timeAfterDash > 0)
        {
            timeAfterDash -= Time.deltaTime;
            if (timeAfterDash < 0)
                timeAfterDash = 0;
        }

        // Make the user dash
        // Note: if for whatever reason the user dashes and jumps in the same frame, I want the jump to take priority
        if (jumped)
            dashed = false;

        if (dashed)
        {
            timeAfterDash = dashTimeLength;
            movement = dashSpeed;
            if (!facingRight)
                movement *= -1;
            rb.gravityScale = 0;
            rb.velocity = new Vector2(movement, 0);
            if (!IsGrounded() && !(NextToWall()))
                dashesLeft--;
        }

        // Make the user go up when they jump, and decrease the jumpsLeft count
        if (jumped)
        {
            if (NextToWall() && wallClimb)
            {
                timeAfterWallJump = wallJumpCooldown;
                if (NextToLeftWall())
                    movement = horizontalJumpSpeed;
                if (NextToRightWall())
                    movement = -horizontalJumpSpeed;

                if (MovingPlatformWall())
                    movement += ((MovingPlatformController)(MovingPlatformWall().GetComponentInParent(typeof(MovingPlatformController)))).Velocity().x;
            }
            rb.velocity = new Vector2(movement, jumpHeight);
            jumped = false;
            if (!IsGrounded())
                jumpsLeft--;
        }
        else
        {
            // The speed at which a vertically moving platform may be moving at
            float platformSpeed = 0f;
            float speedLimit = -wallSlideSpeed;
            // If we are on a moving platform's side
            if (MovingPlatformWall() && wallClimb)
            {
                // Very similar to what we did with movement
                platformSpeed = ((MovingPlatformController)(MovingPlatformWall().GetComponentInParent(typeof(MovingPlatformController)))).Velocity().y;
                speedLimit += platformSpeed;
            }
            if ((NextToWall() && timeAfterWallJump <= 0) && wallClimb)
            {
                // Remember, we are sliding in the negative y direction
                if (rb.velocity.y < speedLimit)
                {
                    rb.velocity = new Vector2(movement, speedLimit);
                }
                else
                {
                    rb.velocity = new Vector2(movement, rb.velocity.y);
                }
            }

            else
            {
                rb.velocity = new Vector2(movement, rb.velocity.y);
            }
        }

        // If we are dashing
        if (timeAfterDash > 0)
        {
            rb.gravityScale = 0;
            rb.velocity = new Vector2(movement, 0);
        }
        else
        {
            rb.gravityScale = gravity;
        }

        // reset jumped and dashed
        if (jumped)
            jumped = false;

        if (dashed)
            dashed = false;
    }

    #region dying and respawing
    public void Die()
    {
        Respawn();
    }

    public void Respawn()
    {
        transform.position = checkpoint;

        // reset everything
        rb.velocity = new Vector3(0, 0, 0);
        jumpsLeft = 0;
        timeAfterWallJump = 0;
        dashesLeft = 0;
        timeAfterDash = 0;
    }
    #endregion

    #region wall checks
    // true if the player is standing on the ground
    private bool IsGrounded()
    {
        float margin = groundMargin;
        Vector2 point = collider.bounds.center;
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(point, collider.bounds.size, 0f, Vector2.down, margin, platformLayerMask);
        bool grounded = raycastHit2D.collider ? raycastHit2D.collider.CompareTag("Platform") || raycastHit2D.collider.CompareTag("Moving Platform") : false;

        return grounded;
    }

    // true if there is a wall next to the player
    private bool NextToWall()
    {
        return NextToLeftWall() || NextToRightWall();
    }

    // true if there is a wall on the player's left
    private bool NextToLeftWall()
    {
        float margin = wallClimbMargin;
        Vector2 point = collider.bounds.center;
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(point, BoxSizeVerticalSmaller(), 0f, Vector2.left, margin, platformLayerMask);
        bool wall = raycastHit2D.collider ? raycastHit2D.collider.CompareTag("Platform") || raycastHit2D.collider.CompareTag("Moving Platform") : false;

        return wall;
    }

    // true if there is a wall on the player's right
    private bool NextToRightWall()
    {
        float margin = wallClimbMargin;
        Vector2 point = collider.bounds.center;
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(point, BoxSizeVerticalSmaller(), 0f, Vector2.right, margin, platformLayerMask);
        bool wall = raycastHit2D.collider ? raycastHit2D.collider.CompareTag("Platform") || raycastHit2D.collider.CompareTag("Moving Platform") : false;

        return wall;
    }

    // Similar to IsGrounded(), but only true for moving platforms
    private bool OnMovingPlatform()
    {
        float margin = groundMargin;
        Vector2 point = collider.bounds.center;
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(point, collider.bounds.size, 0f, Vector2.down, margin, platformLayerMask);
        bool grounded = raycastHit2D.collider ? raycastHit2D.collider.CompareTag("Moving Platform") : false;

        return grounded;
    }
    #endregion

    // Gets the size of the collider, with the top edges a little shaved off so we don't think the platform above us is a wall we are attached to
    private Vector3 BoxSizeVerticalSmaller()
    {
        float margin = verticalMargin;
        Vector3 box = collider.bounds.size;
        box.y = box.y - 2 * margin;
        return box;
    }

    #region moving platforms
    // Gets the platform that we are next to
    private GameObject MovingPlatform()
    {
        RaycastHit2D raycastHit2Ddown = MovingPlatformRayDown();
        RaycastHit2D raycastHit2Dwall = MovingPlatformRayWall();

        // If there is a platform to your left, it return that. Otherwise, if there is a platform to your right, return that. Otherwise, return the platform below you if it exists. Otherwise, return null.
        // If wallClimb is off, we don't care about the platforms to our left or right.

        if (raycastHit2Dwall && raycastHit2Dwall.collider.CompareTag("Moving Platform") && wallClimb)
        {
            return raycastHit2Dwall.collider.gameObject;
        }
        else if (raycastHit2Ddown && raycastHit2Ddown.collider.CompareTag("Moving Platform"))
        {
            return raycastHit2Ddown.collider.gameObject;
        }
        else
        {
            // We are assuming that when this function is called, there is a moving platform next to us. Otherwise, we will get a null pointer error.
            return null;
        }

    }

    // Gets the moving platfrom wall we are next to
    private GameObject MovingPlatformWall()
    {
        RaycastHit2D raycastHit2Dwall = MovingPlatformRayWall();

        if (raycastHit2Dwall && raycastHit2Dwall.collider.CompareTag("Moving Platform") && wallClimb)
        {
            return raycastHit2Dwall.collider.gameObject;
        }
        else
        {
            // We are assuming that when this function is called, there is a moving platform next to us. Otherwise, we will get a null pointer error.
            return null;
        }
    }

    // Helper methods
    private RaycastHit2D MovingPlatformRayDown()
    {
        float margin = groundMargin;
        Vector2 point = collider.bounds.center;
        return Physics2D.BoxCast(point, collider.bounds.size, 0f, Vector2.down, margin, platformLayerMask);
    }

    // Looser margins here to ensure you get a better grip
    private RaycastHit2D MovingPlatformRayLeft()
    {
        float margin = wallClimbMargin;
        Vector2 point = collider.bounds.center;
        return Physics2D.BoxCast(point, BoxSizeVerticalSmaller(), 0f, Vector2.left, margin, platformLayerMask);
    }

    private RaycastHit2D MovingPlatformRayRight()
    {
        float margin = wallClimbMargin;
        Vector2 point = collider.bounds.center;
        return Physics2D.BoxCast(point, BoxSizeVerticalSmaller(), 0f, Vector2.right, margin, platformLayerMask);
    }

    private RaycastHit2D MovingPlatformRayWall()
    {
        RaycastHit2D raycastHit2Dleft = MovingPlatformRayLeft();
        RaycastHit2D raycastHit2Dright = MovingPlatformRayRight();
        if (raycastHit2Dleft)
            return raycastHit2Dleft;
        return raycastHit2Dright;
    }
    #endregion

    //Detect when the player runs into something
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // case if it runs into a power-up
        if (collision.CompareTag("Power-up"))
        {
            Debug.Log("You hit a power-up");

            GameManager.currentPowerUp = GameManager.powerUpOptions[UnityEngine.Random.Range(0, GameManager.powerUpOptions.Length)];
            Debug.Log(GameManager.currentPowerUp);

            collision.gameObject.SendMessage("Collect"); // collision.gameObject gets the game object the Collider 2D is attached to (the power-up)
                                                         // gameObject then calls the function Collect() by using its own sendMessage() method
        }

        else if (collision.CompareTag("Obstacle"))
        {
            Debug.Log("You died!");
            Die();
        }

    }

    private void UsePower()
    {
        Debug.Log("Power used pew pew");

    }

    public bool getIsPlayerOne()
    {
        return isPlayerOne;
    }

    public void stopTime()
    {

    }
}