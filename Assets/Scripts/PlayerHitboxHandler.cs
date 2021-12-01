using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitboxHandler : MonoBehaviour
{
    public GameManager game;

    private void OnTriggerStay2D(Collider2D collision)
    {
        PlayerMovement player = transform.parent.GetComponent<PlayerMovement>();
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.transform.parent.GetComponent<Enemy>();
            if (!enemy.hurt && player.STATE == "SLASH" && collision.gameObject.layer == 7 && gameObject.layer == 6 && enemy.vulnerable) // 7: Hurtbox
            {
                player.currentSlashAmount++;
                game.HandleEnemyDeath();
                enemy.hurt = true;
            }
            else if (player.STATE != "SLASH" && collision.gameObject.layer == 6 && gameObject.layer == 7 && enemy.vulnerable) // 6: Hitbox
            {
                enemy.hit = true;
                if (!player.invincible)
                    player.hurt = true;
            }
        }
    }
}
