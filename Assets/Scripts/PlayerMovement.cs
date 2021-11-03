using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private float groundSlashBufferLength;
    private float slashTimeCount;
    private float groundSlashBufferCount;
    private Vector2 slashDirection;
    [System.NonSerialized] public float currentSlashAmount;

    [SerializeField] private int health;
    [SerializeField] private float invincibilityFrames;
    private int currentHealth;
    private float currentInvincibilityFrames;
    private bool airStunned;
    public bool invincible { get; private set; }
    [System.NonSerialized] public bool hit;

    private bool controlsEnabled;
    public string STATE { get; private set; } //POTENTIAL STATE: "IDLE", "SLASH", "FALL"

    [SerializeField] private GameObject aimLine;
    [SerializeField] private Text slashText;
    [SerializeField] private Text healthText;
    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private Animator animator;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        currentSlashAmount = slashAmount;
        currentHealth = health;
        STATE = "IDLE";
    }

    private void Update()
    {
        if (STATE == "IDLE")
        {
            controlsEnabled = true;
            RenderAimline(true);
        }

        if (STATE == "SLASH")
        {
            controlsEnabled = false;
            RenderAimline(false);
            Slash();
            slashTimeCount += Time.deltaTime;
            if (slashTimeCount >= slashTime)
            {
                animator.SetBool("IsSlashing", false);
                body.velocity = Vector2.zero;
                slashDirection = Vector2.zero;
                STATE = "FALL";
            }
        }

        if (STATE == "FALL")
        {
            controlsEnabled = true;
            RenderAimline(currentSlashAmount > 0);
            if (grounded)
            {
                currentSlashAmount = slashAmount;
                STATE = "IDLE";
            }
        }

        if (hit)
        {
            currentInvincibilityFrames = invincibilityFrames;
            currentHealth--;
            invincible = true;
            airStunned = true;
            hit = false;
            body.velocity = new Vector2(-body.velocity.x / 2, minJumpSpeed);
        }

        if (airStunned)
        {
            RenderAimline(false);
            if (grounded)
                airStunned = false;
        }

        if (!airStunned && currentInvincibilityFrames > 0)
        {
            currentInvincibilityFrames -= Time.deltaTime;
            if (currentInvincibilityFrames <= 0)
            {
                invincible = false;
                currentInvincibilityFrames = 0;
            }
        }

        if (airStunned)
            controlsEnabled = false;

        if (invincible)
            sprite.color = Color.red;
        else
            sprite.color = Color.white;

        if (controlsEnabled)
        {
            // horizontal movement
            int left = Input.GetKey(KeyCode.A) ? 1 : 0;
            int right = Input.GetKey(KeyCode.D) ? 1 : 0;
            int direction = right - left;

            body.velocity = new Vector2(walkSpeed * direction, body.velocity.y);
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
                animator.SetBool("IsJumping", true);
                body.velocity = new Vector2(body.velocity.x, maxJumpSpeed);
                jumpBufferCount = 0;
            }

            if (Input.GetButtonUp("Jump") && body.velocity.y > minJumpSpeed)
            {
                body.velocity = new Vector2(body.velocity.x, minJumpSpeed);
            }

            if (grounded || STATE == "SLASH")
                animator.SetBool("IsJumping", false);
            else
                animator.SetBool("IsJumping", true);
        }
        //Slashing
        if (Input.GetMouseButtonDown(0))
            groundSlashBufferCount = groundSlashBufferLength;
        else
            groundSlashBufferCount -= Time.deltaTime;

        if (groundSlashBufferCount >= 0 && STATE != "SLASH" && currentSlashAmount > 0 && controlsEnabled)
        {
            Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 slashAngle = target - new Vector2(transform.position.x, transform.position.y);
            slashDirection = slashAngle.normalized;
            currentSlashAmount--;
            slashTimeCount = 0;
            RenderAimline(true);
            STATE = "SLASH";
        }

        //Sprite flipping
        sprite.flipX = body.velocity.x < 0 || slashDirection.x * slashSpeed < 0;

        if (currentSlashAmount > slashAmount)
            currentSlashAmount = slashAmount;

        slashText.text = $"Slashes: {currentSlashAmount}/{slashAmount}";
        healthText.text = $"Health: {currentHealth}/{health}";

        if (currentHealth <= 0)
        {
            Destroy(this.gameObject);
        }

        ManageGravity();

    }

    private void Slash()
    {
        animator.SetBool("IsSlashing", true);
        body.velocity = slashDirection * slashSpeed;
    }

    private void ManageGravity()
    {
        if (STATE == "SLASH")
            body.gravityScale = 0f;
        else if (STATE == "FALL" && currentSlashAmount > 0)
            body.gravityScale = 2.5f;
        else
            body.gravityScale = 5f;
    }

    private void RenderAimline(bool enable)
    {
        if (enable)
        {
            aimLine.SetActive(true);
            Vector2 aimVector = Camera.main.WorldToScreenPoint(transform.position);
            Vector2 aimPos = Input.mousePosition;
            aimVector = aimPos - aimVector;
            float aimAngle = Mathf.Atan2(aimVector.y, aimVector.x) * Mathf.Rad2Deg;

            aimLine.transform.position = transform.position;
            aimLine.transform.rotation = Quaternion.AngleAxis(aimAngle, Vector3.forward);
        }
        else
            aimLine.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            grounded = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.transform.parent.GetComponent<Enemy>();
            if (!enemy.touched && STATE == "SLASH" && collision.gameObject.layer == 7) // 7: Hurtbox
            {
                currentSlashAmount++;
                enemy.touched = true;
            }
            else if (STATE != "SLASH" && !invincible && collision.gameObject.layer == 6) // 6: Hitbox
            {
                hit = true;
            }
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
