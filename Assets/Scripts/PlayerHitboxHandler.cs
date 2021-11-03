using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitboxHandler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement player = transform.parent.GetComponent<PlayerMovement>();
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.transform.parent.GetComponent<Enemy>();
            if (!enemy.touched && player.STATE == "SLASH" && collision.gameObject.layer == 7 && gameObject.layer == 6) // 7: Hurtbox
            {
                player.currentSlashAmount++;
                enemy.touched = true;
            }
            else if (player.STATE != "SLASH" && !player.invincible && collision.gameObject.layer == 6 && gameObject.layer == 7) // 6: Hitbox
            {
                player.hit = true;
            }
        }
    }
}
