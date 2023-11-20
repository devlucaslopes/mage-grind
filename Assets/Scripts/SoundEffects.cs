using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum ClipName {
    Button, Error, Success, LevelUp
}

public class SoundEffects : MonoBehaviour
{
    [SerializeField] private AudioClip[] Clips;

    private AudioSource _audioSource;

    public static SoundEffects Instance;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            
            _audioSource = GetComponent<AudioSource>();
        }
    }

    private void OnEnable()
    {
        PlayerController.OnLevelUp += PlayerController_OnLevelUp;
    }

    private void OnDisable()
    {
        PlayerController.OnLevelUp -= PlayerController_OnLevelUp;
    }

    public void PlaySFX(ClipName clipName)
    {
        int index = (int) clipName;
        _audioSource.PlayOneShot(Clips[index]);
    }

    private void PlayerController_OnLevelUp(int _)
    {
        PlaySFX(ClipName.LevelUp);
    }
    
}
