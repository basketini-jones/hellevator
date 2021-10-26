using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float walkSpeed;

    [SerializeField] private float minJumpSpeed;
    [SerializeField] private float maxJumpSpeed;
    private bool grounded = false;

    [SerializeField] private float jumpBufferLength;
    private float jumpBufferCount;

    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private Animator animator;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
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

        if(Input.GetButtonUp("Jump") && body.velocity.y > minJumpSpeed)
        {
            body.velocity = new Vector2(body.velocity.x, minJumpSpeed);
        }

        if (grounded)
            animator.SetBool("IsJumping", false);
        else
            animator.SetBool("IsJumping", true);
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
