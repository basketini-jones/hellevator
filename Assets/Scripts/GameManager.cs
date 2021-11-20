using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private AudioManager audioManager;
    public Button restartButton;
    public Button quitButton;

    private void Awake()
    {
        audioManager = gameObject.GetComponent<AudioManager>();
        restartButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }
    }

    public void HandlePlayerDeath()
    {
        audioManager.Play("PlayerDeath");
        restartButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
    }

    public void HandleEnemyDeath()
    {
        audioManager.Play("EnemyDeath");
    }
}
