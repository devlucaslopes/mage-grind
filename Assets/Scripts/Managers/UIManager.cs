using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Health Bar")]
    [SerializeField] private Slider HealthBarSlider;
    [SerializeField] private TextMeshProUGUI HealthLabel;

    [Header("Experience Bar")] 
    [SerializeField] private Slider ExperienceBarSlider;
    [SerializeField] private TextMeshProUGUI Level;
    
    [Header("Gems")] 
    [SerializeField] private TextMeshProUGUI TotalGems;

    [Header("Level Up")] 
    [SerializeField] private GameObject LevelUpPanel;
    [SerializeField] private CanvasGroup LevelUpCG;

    [Header("Cards")] 
    [SerializeField] private Button CardAttackButton;
    [SerializeField] private Button CardHealthButton;
    [SerializeField] private Button CardMoveSpeedButton;

    [Header("Wave")] 
    [SerializeField] private TextMeshProUGUI CurrentWave;
    [SerializeField] private Slider WaveTimerSlider;

    [Header("Boss")] 
    [SerializeField] private Slider BossHealthBar;

    [Header("Groups")]
    [SerializeField] private GameObject GameplayContainer;

    [Header("Cursor")] 
    [SerializeField] private Image Cursor;
    [SerializeField] private Sprite IconAim;
    [SerializeField] private Sprite IconArrow;

    public static Action<PlayerAttributes.PlayerStats> OnStatsUpgraded;
    
    public static UIManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        GameplayContainer.SetActive(false);
        WaveTimerSlider.gameObject.SetActive(false);

        UpdateTotalGems();
    }

    private void OnEnable()
    {
        CardAttackButton.onClick.AddListener(OnAttackUpgraded);
        CardHealthButton.onClick.AddListener(OnHealthUpgraded);
        CardMoveSpeedButton.onClick.AddListener(OnMoveSpeedUpgraded);
        
        WavesManager.OnWaveStarted += WavesManager_OnWaveStarted;
        PlayerController.OnDie += PlayerController_OnDie;
        PlayerController.OnLevelUp += PlayerController_OnLevelUp;
        GameManager.OnGameStarted += GameManager_OnGameStarted;
        WavesManager.OnBossSpawned += WavesManager_OnBossSpawned;
        PlayerController.OnGemsCollected += PlayerController_OnGemsCollected;
    }

    private void OnDisable()
    {
        CardAttackButton.onClick.RemoveListener(OnAttackUpgraded);
        CardHealthButton.onClick.RemoveListener(OnHealthUpgraded);
        CardMoveSpeedButton.onClick.RemoveListener(OnMoveSpeedUpgraded);
        
        WavesManager.OnWaveStarted -= WavesManager_OnWaveStarted;
        PlayerController.OnDie -= PlayerController_OnDie;
        PlayerController.OnLevelUp -= PlayerController_OnLevelUp;
        GameManager.OnGameStarted -= GameManager_OnGameStarted;
        WavesManager.OnBossSpawned -= WavesManager_OnBossSpawned;
        PlayerController.OnGemsCollected -= PlayerController_OnGemsCollected;
    }

    public void UpdatePlayerHealthBar(int totalHealth, int currentHealth)
    {
        float value = (float) currentHealth / (float) totalHealth;
        
        HealthBarSlider.DOValue(value, 0.5f).SetEase(Ease.OutBack);
        HealthLabel.text = $"{currentHealth}/{totalHealth}";
    }
    
    public void UpdateBossHealthBar(int totalHealth, int currentHealth)
    {
        float value = (float) currentHealth / (float) totalHealth;
        
        BossHealthBar.DOValue(value, 0.5f).SetEase(Ease.OutBack);
    }

    public void UpdateExperienceBar(int totalXP, int currentXP)
    {
        float value = (float) currentXP / (float) totalXP;
        ExperienceBarSlider.DOValue(value, 0.5f).SetEase(Ease.OutBack);
    }

    public void HideLevelUpPanel()
    {
        LevelUpCG.interactable = false;
        Cursor.sprite = IconAim;
        
        LevelUpPanel.SetActive(false);
        GameplayContainer.SetActive(true);
    }

    private void UpdateTotalGems()
    {
        TotalGems.text = $"{SaveManager.Instance.GetGems():000}";
    }
    
    private void ShowLevelUpPanel(int level)
    {
        Cursor.sprite = IconArrow;

        Level.text = $"lv {level}";
        
        GameplayContainer.SetActive(false);
        LevelUpPanel.SetActive(true);
        LevelUpCG.interactable = false;

        StartCoroutine(WaitForInteract());
    }

    private IEnumerator WaitForInteract()
    {
        yield return new WaitForSeconds(0.5f);
        LevelUpCG.interactable = true;
        
        GameManager.Instance.ForcePause();
    }

    public void UpdateWaveTimer(float currentTime)
    {
        if (currentTime <= 0)
        {
            WaveTimerSlider.value = 0;
        }
        else
        {
            WaveTimerSlider.value = currentTime;
        }
    }

    private void GameManager_OnGameStarted()
    {
        Cursor.sprite = IconAim;
        
        GameplayContainer.SetActive(true);
        WaveTimerSlider.gameObject.SetActive(true);
    }
    
    private void OnAttackUpgraded() {
        SoundEffects.Instance.PlaySFX(ClipName.Success);
        OnStatsUpgraded?.Invoke(PlayerAttributes.PlayerStats.Attack);
    }
    
    private void OnHealthUpgraded() {
        SoundEffects.Instance.PlaySFX(ClipName.Success);
        OnStatsUpgraded?.Invoke(PlayerAttributes.PlayerStats.Health);
    }
    
    private void OnMoveSpeedUpgraded() {
        SoundEffects.Instance.PlaySFX(ClipName.Success);
        OnStatsUpgraded?.Invoke(PlayerAttributes.PlayerStats.MoveSpeed);
    }

    private void WavesManager_OnWaveStarted(int waveIndex, float duration)
    {
        waveIndex++;
        
        WaveTimerSlider.maxValue = duration;
        WaveTimerSlider.value = duration;
        
        CurrentWave.text = $"Wave {waveIndex:00}";
    }

    private void PlayerController_OnDie()
    {
        Cursor.sprite = IconArrow;
        
        GameplayContainer.SetActive(false);
        WaveTimerSlider.gameObject.SetActive(false);
        BossHealthBar.gameObject.SetActive(false);
    }

    private void PlayerController_OnLevelUp(int level)
    {
        ShowLevelUpPanel(level);
    }

    private void WavesManager_OnBossSpawned()
    {
        CurrentWave.text = "FINAL BOSS!";
        CurrentWave.DOColor(Color.red, .2f).SetLoops(5);
        
        WaveTimerSlider.gameObject.SetActive(false);
        BossHealthBar.gameObject.SetActive(true);
    }

    private void PlayerController_OnGemsCollected()
    {
        UpdateTotalGems();
    }
}
