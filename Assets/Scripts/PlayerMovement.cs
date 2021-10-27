using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float walkSpeed;

    [SerializeField] private float minJumpSpeed;
    [SerializeField] private float maxJumpSpeed;
    private bool grounded = false;

    [SerializeField] private float jumpBufferLength;
    private float jumpBufferCount;

    [SerializeField] private float slashSpeed;
    [SerializeField] private float slashTime;
    [SerializeField] private float slashAmount;
    private float currentSlashAmount;
    private float slashTimeCount;
    private Vector2 slashDirection;
    private string SLASH_STATE = "IDLE";

    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private Animator animator;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        currentSlashAmount = slashAmount;
    }

    private void Update()
    {
        if (SLASH_STATE != "SLASHING")
        {
            // horizontal movement
            int left = Input.GetKey(KeyCode.A) ? 1 : 0;
            int right = Input.GetKey(KeyCode.D) ? 1 : 0;
            int direction = right - left;

            body.velocity = new Vector2(walkSpeed * direction, body.velocity.y);
            sprite.flipX = body.velocity.x < 0;
            animator.SetFloat("Speed", Mathf.Abs(body.velocity.x));

            //Jump buffer
            if (Input.GetButtonDown("Jump"))
            {
                jumpBufferCount = jumpBufferLength;
            }
            else
            {
                jumpBufferCount -= Time.deltaTime;
            }

            //Handle jump inputs
            if (jumpBufferCount >= 0 && grounded)
            {
                body.velocity = new Vector2(body.velocity.x, maxJumpSpeed);
                jumpBufferCount = 0;
            }

            if (Input.GetButtonUp("Jump") && body.velocity.y > minJumpSpeed)
            {
                body.velocity = new Vector2(body.velocity.x, minJumpSpeed);
            }

            if (grounded)
                animator.SetBool("IsJumping", false);
            else
                animator.SetBool("IsJumping", true);
        }
        if (SLASH_STATE == "SLASHING")
        {
            body.velocity = Vector2.zero;
            Slash();
            slashTimeCount += Time.deltaTime;
            if (slashTimeCount >= slashTime)
            {
                if (currentSlashAmount > 0)
                    body.gravityScale = 2.5f;
                else
                    body.gravityScale = 5f;
                SLASH_STATE = "FALL";
            }
        }
        if (SLASH_STATE == "FALL")
        {
            if (grounded)
            {
                currentSlashAmount = slashAmount;
                body.gravityScale = 5;
                SLASH_STATE = "IDLE";
            }
        }
        //Slashing
        if (Input.GetMouseButtonDown(0) && SLASH_STATE != "SLASHING" && currentSlashAmount > 0)
        {
            Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 angle = target - new Vector2(transform.position.x, transform.position.y);
            slashDirection = angle.normalized;
            body.gravityScale = 0;
            slashTimeCount = 0;
            currentSlashAmount--;
            SLASH_STATE = "SLASHING";
        }
    }

    private void Slash()
    {
        transform.position = new Vector2(transform.position.x, transform.position.y) + slashDirection * slashSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            grounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            grounded = false;
        }
    }
}
