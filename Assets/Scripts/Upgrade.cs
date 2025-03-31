using System;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum UpgradeType { Stamina, DrawSpeed, Power }

[Serializable]
public class Upgrade {
    private const float UPGRADE_EXPONENT = 2.0f;
    public UpgradeType Type;
    public float UpgradeAmount;
    public int MaxUpgrades = 4;
    public int CurrentUpgrades = 0;
    public int Cost = 250;
    public string Description;

    public TMP_Text Readout;
    public TMP_Text Price;
    public Button Button;
    public Sprite Icon;
    public Image Progress;

    public void ApplyUpgrade() {
        if (CurrentUpgrades > MaxUpgrades) {
            return;
        }
        switch (Type) {
            case UpgradeType.Power:
                UpgradeManager.Instance.PowerMultiplier += UpgradeAmount;
                Readout.text = $"{UpgradeManager.Instance.PowerMultiplier:0%}";
                break;
            case UpgradeType.Stamina:
                UpgradeManager.Instance.StaminaMultiplier += UpgradeAmount;
                Readout.text = $"{UpgradeManager.Instance.StaminaMultiplier:0%}";
                break;
            default:
                UpgradeManager.Instance.DrawMultiplier *= UpgradeAmount;
                Readout.text = $"{UpgradeManager.Instance.DrawMultiplier:0%}";
                break;
        }
        CurrentUpgrades++;
        Price.text = GetCost().ToString();
        if (CurrentUpgrades == MaxUpgrades) {
            Readout.text = "MAX";
            Button.gameObject.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public int GetCost() {
        return Mathf.RoundToInt(Cost * Mathf.Pow(UPGRADE_EXPONENT, CurrentUpgrades));
    }
}