using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinalScore : MonoBehaviour
{
    [SerializeField] private bool IsWinScreen;

    [SerializeField] private TextMeshProUGUI Score;
    [SerializeField] private Button PlayAgainButton;
    [SerializeField] private Button MenuButton;

    private int value;
    
    void Start()
    {
        value = IsWinScreen ? SaveManager.Instance.GetWins() : SaveManager.Instance.GetLoses();
        
        string text = Score.text;
        Score.text = text.Replace("{VALUE}", value.ToString());
        
        TransitionManager.Instance.FadeOut();
    }

    private void OnEnable()
    {
        PlayAgainButton.onClick.AddListener(() => OnClickButton(1));
        MenuButton.onClick.AddListener(() => OnClickButton(0));
    }

    private void OnDisable()
    {
        PlayAgainButton.onClick.AddListener(() => OnClickButton(1));
        MenuButton.onClick.AddListener(() => OnClickButton(0));
    }

    private void OnClickButton(int sceneIndex)
    {
        SoundEffects.Instance.PlaySFX(ClipName.Button);
        TransitionManager.Instance.FadeIn(() => SceneManager.LoadScene(sceneIndex));
    }
}
