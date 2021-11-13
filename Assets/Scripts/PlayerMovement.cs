using UnityEngine;
using UnityEngine.UI;
//hullo testy test test test
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

    public float maxCooldownTime;
    private float cooldownTime;

    [SerializeField] private int health;
    [SerializeField] private float invincibilityFrames;
    private int currentHealth;
    private float currentInvincibilityFrames;
    private bool airStunned;
    private bool startBlinking = false;
    private bool deathSoundPlayed = false; //the existence of this variable is my greatest shame as a programmer
    public bool invincible { get; private set; }
    [System.NonSerialized] public bool hit;

    private bool controlsEnabled;
    public string STATE { get; private set; } //POTENTIAL STATE: "IDLE", "SLASH", "COOLDOWN", "FALL", "DEAD"

    [SerializeField] private GameObject aimLine;
    [SerializeField] private Text slashText;
    [SerializeField] private Text healthText;
    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private Animator animator;
    private AudioManager audioManager;
    private AudioSource walkLoopSource;
    public GameManager game;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        walkLoopSource = GetComponent<AudioSource>();
        audioManager = GetComponent<AudioManager>();
        currentSlashAmount = slashAmount;
        currentHealth = health;
        STATE = "IDLE";
        walkLoopSource.enabled = false;
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
                transform.rotation = new Quaternion(0,0,0,0);
                currentInvincibilityFrames = maxCooldownTime;
                STATE = "COOLDOWN";
            }
        }

        if (STATE == "COOLDOWN")
        {
            controlsEnabled = true;
            RenderAimline(false);
            invincible = true;
            currentInvincibilityFrames -= Time.deltaTime;
            groundSlashBufferCount = -1;
            if (currentInvincibilityFrames <= 0)
            {
                invincible = false;
                currentInvincibilityFrames = 0;
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

        if (STATE == "DEAD")
        {
            controlsEnabled = false;
            RenderAimline(false);
            body.velocity = Vector2.zero;
        }

        if (hit)
        {
            currentInvincibilityFrames = invincibilityFrames;
            currentHealth--;
            invincible = true;
            airStunned = true;
            hit = false;
            audioManager.Play("PlayerHurt");
            body.velocity = new Vector2(-body.velocity.x / 2, minJumpSpeed);
            animator.SetBool("IsJumping", false);
            animator.SetTrigger("IsHurt");
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

        if (invincible && !airStunned && currentHealth > 0)
            startBlinking = true;
        else
            startBlinking = false;

        if (startBlinking)
            SpriteBlinkingEffect();
        else
            this.gameObject.GetComponent<SpriteRenderer>().enabled = true;   //make changes

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
                audioManager.Play("PlayerJump");
                body.velocity = new Vector2(body.velocity.x, maxJumpSpeed);
                jumpBufferCount = 0;
            }

            if (Input.GetButtonUp("Jump") && body.velocity.y > minJumpSpeed)
            {
                body.velocity = new Vector2(body.velocity.x, minJumpSpeed);
            }

            if (grounded || STATE == "SLASH")
            {
                animator.SetBool("IsJumping", false);
                animator.SetBool("IsFalling", false);
            }
            else if (STATE == "FALL")
            {
                animator.SetBool("IsJumping", false);
                animator.SetBool("IsFalling", true);
            }
            else
            {
                animator.SetBool("IsJumping", true);
                animator.SetBool("IsFalling", false);
            }
        }
        //Slashing
        if (Input.GetMouseButtonDown(0))
            groundSlashBufferCount = groundSlashBufferLength;
        else
            groundSlashBufferCount -= Time.deltaTime;

        if (groundSlashBufferCount >= 0 && STATE != "SLASH" && STATE != "COOLDOWN" && currentSlashAmount > 0 && controlsEnabled)
        {
            Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 slashAngle = target - new Vector2(transform.position.x, transform.position.y);
            slashDirection = slashAngle.normalized;
            currentSlashAmount--;
            slashTimeCount = 0;
            RenderAimline(true);
            audioManager.Play("PlayerSlash");
            STATE = "SLASH";
        }

        //Sprite flipping
        if (body.velocity.x < 0 || slashDirection.x * slashSpeed < 0)
            sprite.flipX = true;
        else if (body.velocity.x > 0 || slashDirection.x * slashSpeed > 0)
            sprite.flipX = false;

        if (STATE == "SLASH")
            sprite.flipX = false;

        if (airStunned)
            sprite.flipX = !sprite.flipX;

        if (grounded && body.velocity.x != 0 & STATE == "IDLE")
            walkLoopSource.enabled = true;
        else
            walkLoopSource.enabled = false;

        if (currentSlashAmount > slashAmount)
            currentSlashAmount = slashAmount;

        slashText.text = $"Slashes: {currentSlashAmount}/{slashAmount}";
        healthText.text = $"Health: {currentHealth}/{health}";

        animator.SetInteger("Health", currentHealth);
        if (!airStunned && currentHealth <= 0 && !deathSoundPlayed)
        {
            STATE = "DEAD";
            game.HandlePlayerDeath();
            deathSoundPlayed = true;
        }

        ManageGravity();

    }

    private void Slash()
    {
        animator.SetBool("IsSlashing", true);
        body.velocity = slashDirection * slashSpeed;
        Vector2 dir = body.velocity;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 45, Vector3.forward);
    }

    private void ManageGravity()
    {
        if (STATE == "SLASH")
            body.gravityScale = 0f;
        else if (STATE == "FALL" && currentSlashAmount > 0)
            body.gravityScale = 1.5f;
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

    private void SpriteBlinkingEffect()
    {
        if (this.gameObject.GetComponent<SpriteRenderer>().enabled == true)
        {
            this.gameObject.GetComponent<SpriteRenderer>().enabled = false;  //make changes
        }
        else
        {
            this.gameObject.GetComponent<SpriteRenderer>().enabled = true;   //make changes
        }
    }

    private void HandleDeath()
        {
            Destroy(this.gameObject);
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
