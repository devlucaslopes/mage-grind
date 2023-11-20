using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Player Input")]
    [SerializeField] private PlayerInput Input;
    
    [Header("Game Over")]
    [SerializeField] private Button TryAgainButton;
    [SerializeField] private Button ExitButton;

    private InputActionMap gameplayActions;

    private bool _isPaused;

    public bool IsPaused() => _isPaused;

    public static Action OnGameStarted;

    public static GameManager Instance;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        
        Time.timeScale = 1;
    }

    private void Start()
    {
        gameplayActions = Input.actions.FindActionMap("Player");
        
        TransitionManager.Instance.FadeOut(() => OnGameStarted?.Invoke());
    }

    private void OnEnable()
    {
        TryAgainButton.onClick.AddListener(TryAgainAfterGameOver);
        ExitButton.onClick.AddListener(GoToMainMenu);

        UIManager.OnStatsUpgraded += UIManager_OnStatsUpgraded;
        PlayerController.OnDie += PlayerController_OnDie;
    }

    private void OnDisable()
    {
        TryAgainButton.onClick.RemoveListener(TryAgainAfterGameOver);
        ExitButton.onClick.RemoveListener(GoToMainMenu);
        
        UIManager.OnStatsUpgraded -= UIManager_OnStatsUpgraded;
        PlayerController.OnDie -= PlayerController_OnDie;
    }

    private void UIManager_OnStatsUpgraded(PlayerAttributes.PlayerStats _)
    {
        Time.timeScale = 1;
        _isPaused = false;
        
        gameplayActions.Enable();
        
        UIManager.Instance.HideLevelUpPanel();
    }

    private void TryAgainAfterGameOver()
    {
        TransitionManager.Instance.FadeIn(() =>
        {
            TryAgainButton.interactable = false;
            ExitButton.interactable = false;
            SceneManager.LoadScene(1);
        });
    }

    private void GoToMainMenu()
    {
        TransitionManager.Instance.FadeIn(() =>
        {
            TryAgainButton.interactable = false;
            ExitButton.interactable = false;
            SceneManager.LoadScene(0);
        });
    }

    public void ForcePause()
    {
        Time.timeScale = 0;
        _isPaused = true;
        
        gameplayActions.Disable();
    }

    private void PlayerController_OnDie()
    {
        _isPaused = true;
        
        gameplayActions.Disable();
    }
}
