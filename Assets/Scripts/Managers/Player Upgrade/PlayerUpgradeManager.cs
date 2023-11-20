using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerUpgradeManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject PlayerUpgrade;
    [SerializeField] private Button UpgradeButton;
    [SerializeField] private Button CloseUpgradeButton;
    [SerializeField] private Transform UpgradeList;
    [SerializeField] private TextMeshProUGUI PlayerGems;
    [SerializeField] private GameObject UpgradePrefab;

    [Header("Data")] 
    [SerializeField] private PlayerAttributes PlayerData;
    [SerializeField] private PlayerUpgrade[] UpgradeData;
    
    private CanvasGroup playerUpgradeCG;

    private List<UpgradeItem> UpgradeItems = new();

    private void Awake()
    {
        playerUpgradeCG = PlayerUpgrade.GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        int gems = SaveManager.Instance.GetGems();

        PlayerGems.text = gems == 0 ? "You don't have <color #BEF318>gems</color>" : $"You have <color #BEF318>{gems:0000} gems</color>";

        foreach (PlayerUpgrade data in UpgradeData)
        {
            UpgradeItem upgradeItem = Instantiate(UpgradePrefab, UpgradeList).GetComponent<UpgradeItem>();
            upgradeItem.Setup(data);

            Button upgradeItemButton = upgradeItem.GetComponent<Button>();
            
            if (SaveManager.Instance.HasUpgrade(data.name))
            {
                upgradeItemButton.interactable = false;
                upgradeItem.ActiveUpgrade();
            }
            
            UpgradeItems.Add(upgradeItem);
        }
    }

    private void OnEnable()
    {
        UpgradeButton.onClick.AddListener(OnUpgradeOpen);
        CloseUpgradeButton.onClick.AddListener(OnUpgradeClose);

        UpgradeItem.OnPurchasedUpgrade += UpgradeItem_OnPurchasedUpgrade;
    }

    private void OnDisable()
    {
        UpgradeButton.onClick.RemoveListener(OnUpgradeOpen);
        CloseUpgradeButton.onClick.AddListener(OnUpgradeClose);
        
        UpgradeItem.OnPurchasedUpgrade -= UpgradeItem_OnPurchasedUpgrade;
    }
    
    private void OnUpgradeOpen()
    {
        SoundEffects.Instance.PlaySFX(ClipName.Button);
        
        playerUpgradeCG.alpha = 0;
        playerUpgradeCG.interactable = false;
        
        PlayerUpgrade.SetActive(true);

        playerUpgradeCG
            .DOFade(1, 0.3f)
            .OnComplete(() => playerUpgradeCG.interactable = true);
    }

    private void OnUpgradeClose()
    {
        SoundEffects.Instance.PlaySFX(ClipName.Button);
        
        playerUpgradeCG
            .DOFade(0, 0.3f)
            .OnComplete(() =>
            {
                playerUpgradeCG.alpha = 0;
                playerUpgradeCG.interactable = false;
                PlayerUpgrade.SetActive(false);
            });
    }

    private void UpgradeItem_OnPurchasedUpgrade(string upgradeName, int amount)
    {
        int gems = SaveManager.Instance.GetGems();
        PlayerGems.text = gems == 0 ? "You don't have <color #BEF318>gems</color>" : $"You have <color #BEF318>{gems:0000} gems</color>";

        foreach (var upgradeItem in UpgradeItems)
        {
            if (upgradeItem.Name != upgradeName) continue;
            
            upgradeItem.GetComponent<Button>().interactable = false;
            upgradeItem.ActiveUpgrade();
            
            break;
        }
        
        switch (upgradeName)
        {
            case "Damage":
                PlayerData.AddDamage(amount);
                break;
            case "Health":
                PlayerData.AddHealth(amount);
                break;
        }
    }
}
