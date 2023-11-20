using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss : Enemy
{
    [Header("Boss Elements")]
    [SerializeField] private Transform Weapon;
    [SerializeField] private GameObject ImpactEffectPrefab;
    [SerializeField] private ParticleSystem ImpactParticles;

    public void CreateImpact()
    {
        ImpactParticles.Play();
        
        Vector3 position = Weapon.position;
        position.y = 0.1f;
        
        GameObject impact = Instantiate(ImpactEffectPrefab, position, ImpactEffectPrefab.transform.rotation);
        Destroy(impact, 2f);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        
        UIManager.Instance.UpdateBossHealthBar(Health, _currentHealth);
    }

    protected override void BeforeDestroying()
    {
        base.BeforeDestroying();
        
        SaveManager.Instance.AddWin();
        SceneManager.LoadScene(2);
    }
}
