using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    private float waveCount;
    public float maxWaveTime;
    private float waveTime;

    [SerializeField] private GameObject[] enemies;
    [SerializeField] private Text waveCountText;
    [SerializeField] private Text waveTimerText;

    private void Awake()
    {
        waveCount = 0;
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
                float spawnY = Random.Range(Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height / 3)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y);
                float spawnX = Random.Range(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x);

                int selectedEnemy = Random.Range(0, enemies.Length);
                Instantiate(enemies[selectedEnemy], new Vector2(spawnX, spawnY), new Quaternion(0, 0, 0, 0));
            }
            waveCount++;
            waveTime = maxWaveTime;
        }

        waveCountText.text = $"Wave: {waveCount}";
        waveTimerText.text = $"Time: {Mathf.Floor(waveTime)+1}";
    }
}
