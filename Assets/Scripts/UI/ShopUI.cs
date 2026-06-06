using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using PitaRunner.Core;
using PitaRunner.Economy;

namespace PitaRunner.UI
{
    public class ShopUI : MonoBehaviour
    {
        [Header("Tabs")]
        [SerializeField] private GameObject upgradesPanel;
        [SerializeField] private GameObject skinsPanel;

        [Header("Upgrades")]
        [SerializeField] private Transform upgradeContainer;
        [SerializeField] private GameObject upgradeItemPrefab;

        [Header("Skins")]
        [SerializeField] private Transform skinsContainer;
        [SerializeField] private GameObject skinItemPrefab;

        [Header("Coin Display")]
        [SerializeField] private TextMeshProUGUI coinText;

        private void Start()
        {
            UpdateCoinDisplay();
            ShowUpgrades();
        }

        public void ShowUpgrades()
        {
            upgradesPanel?.SetActive(true);
            skinsPanel?.SetActive(false);
            PopulateUpgrades();
        }

        public void ShowSkins()
        {
            upgradesPanel?.SetActive(false);
            skinsPanel?.SetActive(true);
            PopulateSkins();
        }

        private void UpdateCoinDisplay()
        {
            if (coinText != null)
                coinText.text = (SaveManager.Instance?.GetCoins() ?? 0).ToString("N0");
        }

        private void PopulateUpgrades()
        {
            if (upgradeContainer == null || upgradeItemPrefab == null) return;
            foreach (Transform child in upgradeContainer)
                Destroy(child.gameObject);

            var upgrades = UpgradeManager.Instance?.GetAllUpgrades();
            if (upgrades == null) return;

            foreach (var config in upgrades)
            {
                var item = Instantiate(upgradeItemPrefab, upgradeContainer);
                var nameText = item.transform.Find("Name")?.GetComponent<TextMeshProUGUI>();
                var levelText = item.transform.Find("Level")?.GetComponent<TextMeshProUGUI>();
                var costText = item.transform.Find("Cost")?.GetComponent<TextMeshProUGUI>();
                var descText = item.transform.Find("Description")?.GetComponent<TextMeshProUGUI>();
                var upgradeBtn = item.GetComponentInChildren<Button>();

                int level = UpgradeManager.Instance.GetLevel(config.id);
                int cost = UpgradeManager.Instance.GetCost(config.id);
                bool maxed = level >= config.maxLevel;

                if (nameText != null) nameText.text = config.displayName;
                if (levelText != null) levelText.text = $"Nível {level}/{config.maxLevel}";
                if (costText != null) costText.text = maxed ? "MAX" : $"{cost} moedas";
                if (descText != null) descText.text = config.description;
                if (upgradeBtn != null)
                {
                    upgradeBtn.interactable = !maxed && UpgradeManager.Instance.CanUpgrade(config.id);
                    string id = config.id;
                    upgradeBtn.onClick.AddListener(() => OnUpgrade(id));
                }
            }
        }

        private void PopulateSkins()
        {
            if (skinsContainer == null || skinItemPrefab == null) return;
            foreach (Transform child in skinsContainer)
                Destroy(child.gameObject);

            var skins = Skins.SkinManager.Instance?.GetAllSkins();
            if (skins == null) return;

            foreach (var skin in skins)
            {
                var item = Instantiate(skinItemPrefab, skinsContainer);
                var nameText = item.transform.Find("Name")?.GetComponent<TextMeshProUGUI>();
                var costText = item.transform.Find("Cost")?.GetComponent<TextMeshProUGUI>();
                var thumbnail = item.transform.Find("Thumbnail")?.GetComponent<Image>();
                var actionBtn = item.GetComponentInChildren<Button>();
                var btnText = actionBtn?.GetComponentInChildren<TextMeshProUGUI>();

                bool owned = Skins.SkinManager.Instance.IsUnlocked(skin.id);
                bool equipped = SaveManager.Instance?.GetEquippedSkin() == skin.id;

                if (nameText != null) nameText.text = skin.displayName;
                if (costText != null) costText.text = owned ? (equipped ? "EQUIPADO" : "EQUIPAR") : $"{skin.cost} moedas";
                if (thumbnail != null && skin.thumbnail != null) thumbnail.sprite = skin.thumbnail;
                if (actionBtn != null)
                {
                    if (btnText != null) btnText.text = equipped ? "✓" : (owned ? "Equipar" : "Comprar");
                    actionBtn.interactable = !equipped;
                    string id = skin.id;
                    actionBtn.onClick.AddListener(() => OnSkinAction(id));
                }
            }
        }

        private void OnUpgrade(string id)
        {
            AudioManager.Instance?.PlayButtonClick();
            bool success = UpgradeManager.Instance.TryUpgrade(id);
            if (success)
            {
                UpdateCoinDisplay();
                PopulateUpgrades();
            }
        }

        private void OnSkinAction(string id)
        {
            AudioManager.Instance?.PlayButtonClick();
            if (Skins.SkinManager.Instance.IsUnlocked(id))
            {
                Skins.SkinManager.Instance.Equip(id);
            }
            else
            {
                bool purchased = Skins.SkinManager.Instance.TryPurchase(id);
                if (purchased)
                {
                    Skins.SkinManager.Instance.Equip(id);
                    UpdateCoinDisplay();
                }
            }
            PopulateSkins();
        }

        public void OnBackPressed()
        {
            AudioManager.Instance?.PlayButtonClick();
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}
