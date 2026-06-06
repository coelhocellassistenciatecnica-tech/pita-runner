using UnityEngine;
using System.Collections.Generic;
using PitaRunner.Core;

namespace PitaRunner.Skins
{
    [System.Serializable]
    public class SkinConfig
    {
        public string id;
        public string displayName;
        public int cost;
        public bool isFree;
        public GameObject prefab;
        public Sprite thumbnail;
        public Color primaryColor;
        public string description;
    }

    public class SkinManager : MonoBehaviour
    {
        public static SkinManager Instance { get; private set; }

        [Header("Skins")]
        [SerializeField] private List<SkinConfig> allSkins = new List<SkinConfig>
        {
            new SkinConfig { id = "Soldier",   displayName = "Soldado",   cost = 0,    isFree = true,  primaryColor = new Color(0.8f, 0.4f, 0.1f), description = "O clássico soldado do Pita Runner." },
            new SkinConfig { id = "Ninja",     displayName = "Ninja",     cost = 500,  isFree = false, primaryColor = new Color(0.2f, 0.1f, 0.3f), description = "Rápido e silencioso." },
            new SkinConfig { id = "Knight",    displayName = "Cavaleiro", cost = 800,  isFree = false, primaryColor = new Color(0.8f, 0.7f, 0.1f), description = "Armadura de ouro brilhante." },
            new SkinConfig { id = "Robot",     displayName = "Robô",      cost = 1200, isFree = false, primaryColor = new Color(0.5f, 0.7f, 0.9f), description = "Tecnologia do futuro." },
            new SkinConfig { id = "Futuristic",displayName = "Futurista", cost = 1500, isFree = false, primaryColor = new Color(0.1f, 0.9f, 0.8f), description = "Vindo do ano 3000." },
            new SkinConfig { id = "Military",  displayName = "Militar",   cost = 600,  isFree = false, primaryColor = new Color(0.3f, 0.4f, 0.2f), description = "Treinamento de elite." },
        };

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public List<SkinConfig> GetAllSkins() => allSkins;

        public SkinConfig GetSkin(string id)
        {
            return allSkins.Find(s => s.id == id);
        }

        public SkinConfig GetEquippedSkin()
        {
            string id = SaveManager.Instance?.GetEquippedSkin() ?? "Soldier";
            return GetSkin(id) ?? allSkins[0];
        }

        public bool IsUnlocked(string id) => SaveManager.Instance?.IsSkinUnlocked(id) ?? id == "Soldier";

        public bool TryPurchase(string id)
        {
            var skin = GetSkin(id);
            if (skin == null || IsUnlocked(id)) return false;
            if (!Battle.CurrencyManager.Instance.SpendCoins(skin.cost)) return false;
            SaveManager.Instance?.UnlockSkin(id);
            AudioManager.Instance?.PlayCoin();
            return true;
        }

        public void Equip(string id)
        {
            if (!IsUnlocked(id)) return;
            SaveManager.Instance?.SetEquippedSkin(id);
        }

        public Color GetEquippedColor()
        {
            return GetEquippedSkin()?.primaryColor ?? Color.white;
        }
    }
}
