using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wizard Grind/Player/New Attributes", fileName = "PlayerAttributes")]
public class PlayerAttributes : ScriptableObject
{
    public enum PlayerStats
    {
        Attack, Health, MoveSpeed
    };
    
    [Header("Base")]
    [SerializeField] private int Health;
    [SerializeField] private float TimeToRecovery;
    [SerializeField] private float MoveSpeed;
    [SerializeField] private int Damage;
    [SerializeField] private float AttackSpeed;
    [SerializeField] private float ProjectileSpeed;
    [SerializeField] private float XPCollectorRadius;

    [Header("Upgrades")]
    [SerializeField] private int HealthUpgrade;
    [SerializeField] private int DamageUpgrade;
    [SerializeField] private float XPCollectorUpgrade;

    public int GetHealth() => Health;
    public int GetDamage() => Damage;
    public float GetMoveSpeed() => MoveSpeed;
    public float GetTimeToRecovery() => TimeToRecovery;
    public float GetAttackSpeed() => AttackSpeed;
    public float GetProjectileSpeed() => ProjectileSpeed;
    public float GetXPCollectorRadius() => XPCollectorRadius;

    public void AddDamage(int amount)
    {
        Damage += amount;
    }
    
    public void AddHealth(int amount)
    {
        Health += amount;
    }
}
