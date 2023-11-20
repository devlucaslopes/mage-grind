using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class WaveSettings
{
    public int TotalEnemies;
    public Enemy[] Enemies;
    public float TimeBetweenSpawns;
    public float Duration;
}

public class WavesManager : MonoBehaviour
{
    [SerializeField] private List<WaveSettings> Waves;
    [SerializeField] private List<Transform> SpawnPoints;
    [SerializeField] private Transform Player;
    [SerializeField] private Enemy BossPrefab;
    
    private int _waveIndex;
    private WaveSettings _currentWave;
    private float _timer;
    private bool _started;

    public static Action<int, float> OnWaveStarted;
    public static Action OnBossSpawned;

    private void OnEnable()
    {
        GameManager.OnGameStarted += GameManager_OnGameStarted;
        PlayerController.OnDie += PlayerController_OnDie;
    }

    private void OnDisable()
    {
        GameManager.OnGameStarted -= GameManager_OnGameStarted;
        PlayerController.OnDie -= PlayerController_OnDie;
    }

    private void Update()
    {
        if (!_started) return;
        
        _timer -= Time.deltaTime;
        UIManager.Instance.UpdateWaveTimer(_timer);
        
        if (_timer <= 0 || GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            if (_waveIndex + 1 < Waves.Count)
            {
                _waveIndex++;
                StartNextWave(_waveIndex);
            } else
            {
                _started = false;

                StartCoroutine(StartBossFight());
            }
        }
    }
    
    private void StartNextWave(int index)
    {
        _currentWave = Waves[index];
        _timer = _currentWave.Duration;
        
        StartCoroutine(SpawnWave());
        
        OnWaveStarted?.Invoke(index, _currentWave.Duration);
    }
    
    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < _currentWave.TotalEnemies; i++)
        {
            if (Player == null)
            {
                yield break;
            }

            Enemy randomEnemy = _currentWave.Enemies[Random.Range(0, _currentWave.Enemies.Length)];
            Transform randomSpot = SpawnPoints[Random.Range(0, SpawnPoints.Count)];

            Enemy enemy = Instantiate(randomEnemy, randomSpot.position, randomSpot.rotation).GetComponent<Enemy>();
            enemy.Setup(Player);

            yield return new WaitForSeconds(_currentWave.TimeBetweenSpawns);
        }
    }

    private IEnumerator StartBossFight()
    {
        OnBossSpawned?.Invoke();

        yield return new WaitForSeconds(1);
        
        Transform randomSpot = SpawnPoints[Random.Range(0, SpawnPoints.Count)];
        Enemy boss = Instantiate(BossPrefab, randomSpot.position, randomSpot.rotation).GetComponent<Enemy>();
        boss.Setup(Player);

    }

    private void GameManager_OnGameStarted()
    {
        _started = true;
        StartNextWave(_waveIndex);
    }

    private void PlayerController_OnDie()
    {
        _started = false;
    }
}
