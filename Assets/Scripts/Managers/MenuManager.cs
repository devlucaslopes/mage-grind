using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button StartButton;

    private void Start()
    {
        TransitionManager.Instance.FadeOut();
    }

    private void OnEnable()
    {
        StartButton.onClick.AddListener(OnStartGame);
    }

    private void OnDisable()
    {
        StartButton.onClick.RemoveListener(OnStartGame);
    }

    private void OnStartGame()
    {
        SoundEffects.Instance.PlaySFX(ClipName.Button);
        TransitionManager.Instance.FadeIn(() => SceneManager.LoadScene(1));
    }
}
