using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // How long (time) we want our dash to last
    public float dashTimeLength;

    // How fast we move while in a dash
    public float dashSpeed;

    // How many dashes the user can do after leaving the ground
    public int numDashes;


    [SerializeField]
    LayerMask platformLayerMask;

    // true if the charater is facing right
    private bool facingRight;

    // We use this for flipping only the graphics of our charater without changing our hitbox or anyhting else
    private Transform playerVisual;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Collider2D collider;
    private Rigidbody2D rb;

    // Horizintal velocity
    private float movement;

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


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        playerVisual = transform.GetChild(0);
        facingRight = true;
        gravity = rb.gravityScale;
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
            movement = Input.GetAxis("Horizontal") * speed * (dashTimeLength-timeAfterDash)/dashTimeLength + dashMovement * timeAfterDash/dashTimeLength;
            
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
        if (IsGrounded() || (NextToWall() && wallClimb))
        {
            jumpsLeft = extraJumps;
            dashesLeft = numDashes;
        }

        // If the user is allowed to jump
        if (!jumped && (jumpsLeft!=0 ||IsGrounded() || (NextToWall() && wallClimb)))
        {
            jumped = Input.GetButtonDown("Jump");
        }

        // Get input for dash
        if (!dashed && timeAfterDash <= 0 && dashesLeft > 0)
        {
            dashed = Input.GetButtonDown("Dash");
        }

        #endregion


        #region animation

        // If facing the wrong way compared to how you are moving, flip the graphic
        if ((movement < 0 && facingRight) || (movement > 0 && !facingRight))
            playerVisual.Rotate(Vector3.up, 180);

        // Updating facingRight
        if (movement > 0)
            facingRight = true;
        if (movement < 0)
            facingRight = false;

        #endregion

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
            }
            rb.velocity = new Vector2(movement, jumpHeight);
            jumped = false;
            if(!IsGrounded() && !(NextToWall() && wallClimb))
                jumpsLeft--;
        }
        else
        {
            if ((NextToWall() && timeAfterWallJump <= 0) && wallClimb)
            {
                rb.velocity = new Vector2(movement, 0);
            }
            else
            {
                rb.velocity = new Vector2(movement, rb.velocity.y);
            }
        }

        // If you are sticking to a wall or dashing
        if (((NextToWall() && timeAfterWallJump <= 0) && wallClimb) || timeAfterDash > 0)
        {
            rb.gravityScale = 0;
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

    // true if the player is standing on the ground
    private bool IsGrounded()
    {
        float margin = 0.05f;
        Vector2 point = collider.bounds.center;
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(point, collider.bounds.size, 0f, Vector2.down, margin, platformLayerMask);
        bool grounded = raycastHit2D.collider ? raycastHit2D.collider.CompareTag("Platform") : false;

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
        float margin = 0.05f;
        Vector2 point = collider.bounds.center;
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(point, collider.bounds.size, 0f, Vector2.left, margin, platformLayerMask);
        bool wall = raycastHit2D.collider ? raycastHit2D.collider.CompareTag("Platform") : false;

        return wall;
    }

    // true if there is a wall on the player's right
    private bool NextToRightWall()
    {
        float margin = 0.05f;
        Vector2 point = collider.bounds.center;
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(point, collider.bounds.size, 0f, Vector2.right, margin, platformLayerMask);
        bool wall = raycastHit2D.collider ? raycastHit2D.collider.CompareTag("Platform") : false;

        return wall;
    }
}
