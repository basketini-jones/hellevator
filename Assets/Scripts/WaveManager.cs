using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    private float waveCount;
    public float maxWaveTime;
    private float waveTime;

    private AudioManager audioManager;

    [SerializeField] private GameObject[] enemies;
    [SerializeField] private Text waveCountText;
    [SerializeField] private Text waveTimerText;
    [SerializeField] private BoxCollider2D spawnZone;

    private void Awake()
    {
        waveCount = 0;
        audioManager = gameObject.GetComponent<AudioManager>();
    }

    private void Update()
    {
        if (waveTime > 0)
        {
            waveTime -= Time.deltaTime;
        }
        
        if ((waveTime <= 0 || GameObject.FindGameObjectsWithTag("Enemy").Length == 0) && GameObject.FindGameObjectsWithTag("Player").Length > 0)
        {
            for (int i = 0; i <= waveCount; i++)
            {
                Bounds bounds = spawnZone.bounds;
                float spawnY = Random.Range(bounds.min.y, bounds.max.y);
                float spawnX = Random.Range(bounds.min.x, bounds.max.x);

                int selectedEnemy = Random.Range(0, enemies.Length);
                Instantiate(enemies[selectedEnemy], new Vector2(spawnX, spawnY), new Quaternion(0, 0, 0, 0));

                audioManager.Play("NewWave");
            }
            waveCount++;
            waveTime = maxWaveTime;
        }

        waveCountText.text = $"{waveCount}";
        waveTimerText.text = $"{Mathf.Floor(waveTime)+1}";
    }
}
