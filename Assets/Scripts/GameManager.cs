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
    public SpriteRenderer black;
    private AudioSource music;

    private void Awake()
    {
        audioManager = gameObject.GetComponent<AudioManager>();
        restartButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        black.gameObject.SetActive(false);
        AudioSource[] audioSources = gameObject.GetComponents<AudioSource>();
        foreach (AudioSource audioSource in audioSources)
        {
            if (audioSource.loop)
                music = audioSource;
        }
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
        black.gameObject.SetActive(true);
        music.mute = true;
    }

    public void HandleEnemyDeath()
    {
        audioManager.Play("EnemyDeath");
    }
}
