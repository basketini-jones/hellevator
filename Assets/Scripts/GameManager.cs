using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = gameObject.GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Instantiate(enemy, new Vector2(2, 1), new Quaternion(0,0,0,0));
            Instantiate(enemy, new Vector2(-2, 1), new Quaternion(0, 0, 0, 0));
        }
    }

    public void HandlePlayerDeath()
    {
        audioManager.Play("PlayerDeath");
    }

    public void HandleEnemyDeath()
    {
        audioManager.Play("EnemyDeath");
    }
}
