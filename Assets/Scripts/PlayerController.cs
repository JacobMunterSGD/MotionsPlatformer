using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] Rigidbody2D rb;

    [SerializeField] float force;
    [SerializeField] float maxSpeed;
    [SerializeField] float groundFriction;

    [SerializeField] float groundCheckDistance;
    [SerializeField] private LayerMask jumpLayerMask;

    [SerializeField] float apexHeight;
    [SerializeField] float apexTime;

    [SerializeField] float jumpVelocity;
    [SerializeField] float gravity;

    [SerializeField] float terminalFallingSpeed;

    float coyoteTimer;
    [SerializeField] float coyoteTimerStartValue;

    float gameTimer;
    float lastJumped;

    FacingDirection lastDirectionFaced;

    public enum FacingDirection
    {
        left, right
    }

    void OnValidate()
    {
        rb = GetComponent<Rigidbody2D>();

        //Physics2D.gravity = new Vector2(0, gravity);

        gravity = -2f * apexHeight / (apexTime * apexTime);
        jumpVelocity = 2 * apexHeight / apexTime;

        gameTimer = 0;
    }

    void FixedUpdate()
    {
        Vector2 playerInput = new Vector2();

        playerInput.x = Input.GetAxisRaw("Horizontal");        

        MovementUpdate(playerInput);

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Jump();
        CoyoteTimerUpdate();
        gameTimer += Time.deltaTime;

    }
    private void MovementUpdate(Vector2 playerInput)
    {

        if (maxSpeed < Mathf.Abs(rb.velocity.x + ((playerInput.x * force) / rb.mass) * Time.deltaTime))
        {
            rb.velocity = new Vector2(maxSpeed * playerInput.x, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x + playerInput.x * force * Time.deltaTime, rb.velocity.y);
        }

        GroundFriction();

        if (-0.1 < rb.velocity.x && rb.velocity.x < 0.1)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        //if (IsGrounded()) rb.gravityScale = 0;
        //else rb.gravityScale = 1;

        if (IsGrounded()) PlayerGravity(0);
        else PlayerGravity(1);

        TerminalFallingVelocityCheck();
    }
    void PlayerGravity(float multiplier)
    {
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + gravity * Time.deltaTime * multiplier);
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
            coyoteTimer = 0;
            lastJumped = gameTimer;
        }
    }

    void TerminalFallingVelocityCheck()
    {
        if (rb.velocity.y + gravity * Time.deltaTime < terminalFallingSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, terminalFallingSpeed);
        }
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
    }

    public bool IsWalking()
    {
        return (Mathf.Abs(rb.velocity.x) > .1);
    }

    public bool IsGrounded()
    {

        RaycastHit2D hitInfoBoxCast
            = Physics2D.BoxCast(transform.position, new Vector2(transform.localScale.x, groundCheckDistance),
            0, Vector2.down, groundCheckDistance, jumpLayerMask);

        if (hitInfoBoxCast.collider != null)
        {
            Debug.DrawLine(transform.position, hitInfoBoxCast.point, Color.green);
        }
        
        return hitInfoBoxCast.collider != null;
        
    }

    public FacingDirection GetFacingDirection()
    {
        if (rb.velocity.x < 0)
        {
            lastDirectionFaced = FacingDirection.left;
        }

        if (rb.velocity.x > 0)
        {
            lastDirectionFaced = FacingDirection.right;
        }

        return lastDirectionFaced;
    }
}
