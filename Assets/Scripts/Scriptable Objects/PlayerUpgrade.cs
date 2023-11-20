using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wizard Grind/Player/New upgrade", fileName = "NewUpgrade")]
public class PlayerUpgrade : ScriptableObject
{
    public enum TypeUpgrade
    {
        Damage, Health, MoveSpeed
    }
    
    [SerializeField] private Sprite Icon;
    [SerializeField] private TypeUpgrade Type;
    [SerializeField] private int Amount;
    [SerializeField] private int Price;

    public Sprite GetIcon() => Icon;
    public TypeUpgrade GetTypeUpgrade() => Type;
    public int GetAmount() => Amount;

    public int GetPrice() => Price;
}
