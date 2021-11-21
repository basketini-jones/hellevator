using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [HideInInspector] public bool hurt;
    [HideInInspector] public bool hit;
    [HideInInspector] public bool vulnerable;
    public bool EnableGravity;
    public bool MoveHorizontally;
    public float MoveSpeed;
    private int direction;

    private Rigidbody2D body;
    private Animator animator;
    private SpriteRenderer sprite;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        hit = false;
        hurt = false;
        vulnerable = false;

        direction = Random.Range(0, 1);
        if (direction == 0) direction = -1;

    }

    // Update is called once per frame
    private void Update()
    {
        if (hurt)
        {
            Destroy(this.gameObject);
        }
        if (hit)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("enemy1_attack"))
                animator.SetTrigger("Attack");
            hit = false;
        }

        if (MoveHorizontally && vulnerable)
        {
            body.velocity = new Vector2(MoveSpeed * direction, body.velocity.y);
            if (direction < 0)
                sprite.flipX = true;
            else
                sprite.flipX = false;
        }
    }

    public void BecomeVulnerable()
    {
        if (!vulnerable)
        {
            if (EnableGravity)
                body.gravityScale = 5f;
            else
                body.gravityScale = 0f;
            vulnerable = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Untagged")
        {
            direction = -direction;
        }
    }
}
