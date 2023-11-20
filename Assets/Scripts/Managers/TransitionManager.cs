using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    [SerializeField] private Image Panel;

    public static TransitionManager Instance;

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
    }

    public void FadeOut(Action onComplete = null)
    {
        Panel
            .DOFade(0, 1)
            .OnComplete(() =>
            {
                if (onComplete != null) onComplete();
                
                Panel.gameObject.SetActive(false);
            });
    }
    
    public void FadeIn(Action onComplete = null)
    {
        Panel.gameObject.SetActive(true);
        
        Panel
            .DOFade(1, 1)
            .OnComplete(() =>
            {
                if (onComplete != null) onComplete();
            });
    }
}
