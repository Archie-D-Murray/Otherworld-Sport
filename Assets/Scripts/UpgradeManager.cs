using System;
using System.Collections.Generic;

using Tags.UI;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using Utilities;

public class UpgradeManager : Singleton<UpgradeManager> {
    private Dictionary<UpgradeType, int> _lookup = new Dictionary<UpgradeType, int>();

    [SerializeField] private int _money;
    [SerializeField] private Upgrade[] _upgrades = new Upgrade[3];
    [SerializeField] private GameObject _upgradePrefab;
    [SerializeField] private CanvasGroup _upgradeCanvas;


    public float PowerMultiplier = 1.0f;
    public float StaminaMultiplier = 1.0f;
    public float DrawMultiplier = 1.0f;

    public int Money { get => _money; set => _money = value; }

    private void Start() {
        for (int i = 0; i < _upgrades.Length; i++) {
            Upgrade upgrade = _upgrades[i];

            _lookup.Add(upgrade.Type, i);
            GameObject instance = Instantiate(_upgradePrefab, _upgradeCanvas.transform);
            instance.GetComponentInChildren<TMP_Text>().text = upgrade.GetCost().ToString();
            foreach (Image image in instance.GetComponentsInChildren<Image>()) {
                if (image.gameObject.HasComponent<IconTag>()) {
                    image.sprite = upgrade.Icon;
                } else if (image.gameObject.HasComponent<ReadoutTag>()) {
                    upgrade.Progress = image;
                    image.fillAmount = 0;
                }
            }
            Button button = instance.GetComponentInChildren<Button>();
            upgrade.Button = button;
            foreach (TMP_Text text in instance.GetComponentsInChildren<TMP_Text>()) {
                if (text.gameObject.HasComponent<ReadoutTag>()) {
                    upgrade.Readout = text;
                    upgrade.Readout.text = "100%";
                } else if (text.gameObject.HasComponent<PriceTag>()) {
                    upgrade.Price = text;
                    upgrade.Price.text = upgrade.GetCost().ToString();
                }
            }
            button.onClick.AddListener(() => Buy(upgrade));
            button.interactable = false;
        }
    }

    private void Buy(Upgrade upgrade) {
        upgrade.ApplyUpgrade();
        UpdateUpgrades();
    }

    private void UpdateUpgrades() {
        Debug.Log("Updated upgrades");
        foreach (Upgrade upgrade in _upgrades) {
            upgrade.Button.interactable = _money >= upgrade.GetCost() && upgrade.CurrentUpgrades < upgrade.MaxUpgrades;
        }
    }
}