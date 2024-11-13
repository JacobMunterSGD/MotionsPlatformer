using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] Rigidbody2D rb;

    [SerializeField] float force;
    [SerializeField] float maxSpeed;

    [SerializeField] float groundCheckDistance;
    [SerializeField] private LayerMask jumpLayerMask;

    FacingDirection lastDirectionFaced;

    public enum FacingDirection
    {
        left, right
    }

    void OnValidate()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // The input from the player needs to be determined and
        // then passed in the to the MovementUpdate which should
        // manage the actual movement of the character.

        Vector2 playerInput = new Vector2();

        playerInput.x = Input.GetAxisRaw("Horizontal");

        MovementUpdate(playerInput);

        //bool temp = IsGrounded();
    }

    private void Update()
    {
        
    }

    private void MovementUpdate(Vector2 playerInput)
    {

        if (maxSpeed < Mathf.Abs(rb.velocity.x + ((playerInput.x * force) / rb.mass) * Time.deltaTime))
        {
            rb.velocity = new Vector2(maxSpeed * playerInput.x, 0);
        }
        else
        {
            rb.AddForce(new Vector2(playerInput.x * force * Time.deltaTime, 0), ForceMode2D.Impulse);
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
