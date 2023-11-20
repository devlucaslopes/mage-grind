using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Animator PlayerCameraAnimator;

    private void OnEnable()
    {
        PlayerController.OnTakeDamage += PlayerController_OnTakeDamage;
    }

    private void OnDisable()
    {
        PlayerController.OnTakeDamage -= PlayerController_OnTakeDamage;
    }

    private void PlayerController_OnTakeDamage(int _)
    {
        PlayerCameraAnimator.SetTrigger("shake");
    }
}
