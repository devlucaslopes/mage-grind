using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAnimator : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private PlayerController PlayerController;

    public void ShootProjectile() => PlayerController.ShootProjectile();

    public void GetHitFinished() => PlayerController.SetCanMove(true);

    public void OnDead()
    {
        TransitionManager.Instance.FadeIn(() => SceneManager.LoadScene(3));
    }
}
