using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeItem : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private Color ActiveColor;
    
    [Header("UI")]
    [SerializeField] private Image Icon;
    [SerializeField] private TextMeshProUGUI Description;
    [SerializeField] private TextMeshProUGUI Price;

    private PlayerUpgrade _data;

    public string Name => _data.name;

    public static Action<string, int> OnPurchasedUpgrade;

    public void OnApplyUpgrade()
    {
        int gems = SaveManager.Instance.GetGems();
        int price = _data.GetPrice();
        
        if (gems < price)
        {
            SoundEffects.Instance.PlaySFX(ClipName.Error);
            return;
        }
        
        SoundEffects.Instance.PlaySFX(ClipName.Success);
        SaveManager.Instance.RemoveGems(price);
        SaveManager.Instance.AddUpgrade(_data.name);
        
        OnPurchasedUpgrade?.Invoke(_data.name, _data.GetAmount());
    }

    public void Setup(PlayerUpgrade data)
    {
        _data = data;
        
        Icon.sprite = _data.GetIcon();
        Description.text = $"+{_data.GetAmount()} {_data.GetTypeUpgrade()}";
        Price.text = $"{_data.GetPrice()}";
    }

    public void ActiveUpgrade()
    {
        Icon.color = ActiveColor;
        Description.color = ActiveColor;
        Price.text = "Actived!";
    }
}
