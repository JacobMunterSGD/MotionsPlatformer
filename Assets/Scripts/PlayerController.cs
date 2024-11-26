using UnityEngine;

public enum PlayerState
{
    idle, walking, jumping, dead
}

public class PlayerController : MonoBehaviour
{

    [SerializeField] Rigidbody2D rb;

    public PlayerState currentState = PlayerState.idle;
    public PlayerState previousState = PlayerState.idle;

    [Header("Horizontal")]
    [SerializeField] float force;
    [SerializeField] float maxSpeed;

    [SerializeField] float accelerationTime;
    [SerializeField] float decelerationTime;

    float accelerationRate;
    float decelerationRate;

    [SerializeField] float groundFriction;

    [Header("Ground Check")]
    [SerializeField] float groundCheckDistance;
    [SerializeField] private LayerMask jumpLayerMask;

    [Header("Vertical")]
    [SerializeField] float apexHeight;
    [SerializeField] float apexTime;

    [SerializeField] float jumpVelocity;
    [SerializeField] float gravity;

    [SerializeField] float terminalFallingSpeed;

    [Header("Coyote Time")]
    float coyoteTimer;
    [SerializeField] float coyoteTimerStartValue;

    float gameTimer;
    float lastJumped;

    FacingDirection lastDirectionFaced;

    Vector2 velocity;

    private bool isDead = false;

    public enum FacingDirection
    {
        left, right
    }

    void OnValidate()
    {
        rb = GetComponent<Rigidbody2D>();

        gravity = -2f * apexHeight / (apexTime * apexTime);
        jumpVelocity = 2 * apexHeight / apexTime;

        gameTimer = 0;

        accelerationRate = maxSpeed / accelerationTime;
        decelerationRate = maxSpeed / decelerationTime;

        rb.gravityScale = 0;

        //print("acc: "+ accelerationRate);
        //print("dec: " + decelerationRate);

    }

    void FixedUpdate()
    {
        Vector2 playerInput = new Vector2();
        playerInput.x = Input.GetAxisRaw("Horizontal");

        MovementUpdate(playerInput);
        //rb.velocity = velocity;

    }
    void Update()
    {
        previousState = currentState;

        if (isDead)
        {
            currentState = PlayerState.dead;
        }

        switch (currentState)
        {
            case PlayerState.dead:
                break;

            case PlayerState.idle:
                if (!IsGrounded()) currentState = PlayerState.jumping;
                else if (rb.velocity.x != 0) currentState = PlayerState.walking;
                break;

            case PlayerState.walking:
                if (!IsGrounded()) currentState = PlayerState.jumping;
                else if (rb.velocity.x == 0) currentState = PlayerState.idle;
                break;

            case PlayerState.jumping:
                if (IsGrounded())
                {
                    if (rb.velocity.x != 0) currentState = PlayerState.walking;
                    else currentState = PlayerState.idle;
                }
                break;
        }

        if (Input.GetKeyDown(KeyCode.Space)) Jump();
        CoyoteTimerUpdate();
        gameTimer += Time.deltaTime;

    }

    //void MovementUpdate2(Vector2 playerInput) // incorporate this into how it works right now
    //{
    //    if (playerInput.x != 0)
    //    {
    //        velocity.x += accelerationRate * playerInput.x * Time.deltaTime;
    //    }
    //    else
    //    {
    //        if (velocity.x > 0)
    //        {
    //            velocity.x -= decelerationRate * Time.deltaTime;
    //            velocity.x = Mathf.Max(velocity.x, 0);
    //        }
    //        else if (velocity.x < 0)
    //        {
    //            velocity.x += decelerationRate * Time.deltaTime;
    //            velocity.x = Mathf.Max(velocity.x, 0);
    //        }
    //    }
    //}

    private void MovementUpdate(Vector2 playerInput)
    {
        Walking(playerInput);

        if (IsGrounded())
        {
            GroundFriction();
            PlayerGravity(0);
            if (rb.velocity.y < 0) rb.velocity = new Vector2(rb.velocity.x, 0);
            // if (velocity.y < 0) velocity.y = 0;
        }
        else
        {
            PlayerGravity(1f);
        }

        MakeXVelocity0IfSmallNumber();

        TerminalFallingVelocityCheck();

        void Walking(Vector2 playerInput)
        {
            if (maxSpeed < Mathf.Abs(rb.velocity.x + playerInput.x * force * Time.deltaTime))
            {
                rb.velocity = new Vector2(maxSpeed * playerInput.x, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x + playerInput.x * force * Time.deltaTime, rb.velocity.y);
            }
            //if (maxSpeed < Mathf.Abs(velocity.x + playerInput.x * force * Time.deltaTime))
            //{
            //    velocity.x = maxSpeed * playerInput.x;
            //}
            //else
            //{
            //    velocity.x = velocity.x + playerInput.x * force * Time.deltaTime;
            //}
        }
        void PlayerGravity(float multiplier)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + gravity * Time.deltaTime * multiplier);
            // velocity.y = velocity.y + gravity * Time.deltaTime * multiplier;
        }
        void GroundFriction()
        {
            if (rb.velocity.x > 0.1 && IsGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x - groundFriction * Time.deltaTime, rb.velocity.y);
            }
            if (rb.velocity.x < -0.1 && IsGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x + groundFriction * Time.deltaTime, rb.velocity.y);
            }
            //if (velocity.x > 0.1 && IsGrounded())
            //{
            //    velocity.x = velocity.x - groundFriction * Time.deltaTime;
            //}
            //if (velocity.x < -0.1 && IsGrounded())
            //{
            //    velocity.x = rb.velocity.x + groundFriction * Time.deltaTime;
            //}
        }
        void TerminalFallingVelocityCheck()
        {
            if (rb.velocity.y + gravity * Time.deltaTime < terminalFallingSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, terminalFallingSpeed);
            }
            //if (velocity.y + gravity * Time.deltaTime < terminalFallingSpeed)
            //{
            //    velocity.y = terminalFallingSpeed;
            //}
        }
        void MakeXVelocity0IfSmallNumber()
        {
            if (-0.1 < rb.velocity.x && rb.velocity.x < 0.1) // 
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            //if (-0.1 < velocity.x && velocity.x < 0.1) // 
            //{
            //    velocity = new Vector2(0, velocity.y);
            //}

        }
    }
    

    void CoyoteTimerUpdate()
    {
        if (!IsGrounded())
        {
            if (coyoteTimer >= 0)
            {
                coyoteTimer -= Time.deltaTime;
            }
        }
        else
        {
            coyoteTimer = coyoteTimerStartValue;
        }
    }

    void Jump()
    {
        bool jumpAvailable = false;

        if (IsGrounded())
        {
            jumpAvailable = true;
        }
        else
        {
            if (coyoteTimer > 0 && gameTimer - lastJumped > coyoteTimerStartValue)
            {
                jumpAvailable = true;
            }
        }

        if (jumpAvailable)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
            // velocity.y = jumpVelocity;
            coyoteTimer = 0;
            lastJumped = gameTimer;
        }
    }

    public bool IsWalking()
    {
        return (Mathf.Abs(rb.velocity.x) > .1);
        //return (Mathf.Abs(velocity.x) > .1);
    }

    public bool IsGrounded()
    {

        RaycastHit2D hitInfoBoxCast
            = Physics2D.BoxCast(transform.position, new Vector2(transform.localScale.x, groundCheckDistance),
            0, Vector2.down, groundCheckDistance, jumpLayerMask);

        DebugGroundCheck();
        
        return hitInfoBoxCast.collider != null;

        void DebugGroundCheck()
        {
            if (hitInfoBoxCast.collider != null)
            {
                Debug.DrawLine(transform.position, hitInfoBoxCast.point, Color.green);
            }
        }
        
    }

    public FacingDirection GetFacingDirection()
    {
        if (rb.velocity.x < 0)
        {
            lastDirectionFaced = FacingDirection.left;
        }

        else if (rb.velocity.x > 0)
        {
            lastDirectionFaced = FacingDirection.right;
        }

        return lastDirectionFaced;
    }
}
