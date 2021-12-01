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
    public float HorizontalMoveSpeed;
    public bool MoveSine;
    public float VerticalMaxSpeed;
    public float VerticalAcceleration;
    public bool PursuePlayer;
    public float PursueSpeed;
    private GameObject player;
    private float verticalMoveSpeed;
    private int horizontalDirection;
    private int verticalDirection;

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

        if (MoveHorizontally)
        {
            horizontalDirection = Random.Range(0, 1);
            if (horizontalDirection == 0) horizontalDirection = -1;
        }
        if (MoveSine)
        {
            verticalMoveSpeed = 0;
            verticalDirection = Random.Range(0, 1);
            if (verticalDirection == 0) verticalDirection = -1;
        }
        if (PursuePlayer)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            player = players[0];
        }

        body.gravityScale = 0f;

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
            body.velocity = new Vector2(HorizontalMoveSpeed * horizontalDirection, body.velocity.y);
            if (horizontalDirection < 0)
                sprite.flipX = true;
            else
                sprite.flipX = false;
        }
        if (MoveSine && vulnerable)
        {
            if (Mathf.Abs(verticalMoveSpeed) >= VerticalMaxSpeed)
            {
                verticalDirection *= -1;
            }
            verticalMoveSpeed += VerticalAcceleration * verticalDirection * Time.fixedDeltaTime;
            body.velocity = new Vector2(body.velocity.x, verticalMoveSpeed);
        }
        if (PursuePlayer && vulnerable && player != null)
        {
            body.velocity = Vector3.Normalize(player.transform.position - transform.position) * PursueSpeed;
            Vector2 aimVector = player.transform.position - transform.position;
            float aimAngle = Mathf.Atan2(aimVector.y, aimVector.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(aimAngle, Vector3.forward);
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
            horizontalDirection = -horizontalDirection;
        }
    }
}
