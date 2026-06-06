using UnityEngine;
using PitaRunner.Core;

namespace PitaRunner.Economy
{
    [System.Serializable]
    public class UpgradeConfig
    {
        public string id;
        public string displayName;
        public string description;
        public int maxLevel;
        public int baseCost;
        public float costMultiplier;

        public int GetCostForLevel(int level) =>
            Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplier, level - 1));
    }

    public class UpgradeManager : MonoBehaviour
    {
        public static UpgradeManager Instance { get; private set; }

        [Header("Upgrade Configs")]
        [SerializeField] private UpgradeConfig damageConfig = new UpgradeConfig
            { id = "damage", displayName = "Dano", description = "+5 dano por nível", maxLevel = 10, baseCost = 100, costMultiplier = 1.5f };
        [SerializeField] private UpgradeConfig speedConfig = new UpgradeConfig
            { id = "speed", displayName = "Velocidade", description = "+10% velocidade", maxLevel = 10, baseCost = 80, costMultiplier = 1.4f };
        [SerializeField] private UpgradeConfig startCountConfig = new UpgradeConfig
            { id = "startCount", displayName = "Qtd. Inicial", description = "+5 unidades iniciais", maxLevel = 10, baseCost = 150, costMultiplier = 1.6f };
        [SerializeField] private UpgradeConfig multiplyRateConfig = new UpgradeConfig
            { id = "multiplyRate", displayName = "Taxa Multiplicadora", description = "+10% efetividade dos portais", maxLevel = 10, baseCost = 200, costMultiplier = 1.7f };
        [SerializeField] private UpgradeConfig unitHealthConfig = new UpgradeConfig
            { id = "unitHealth", displayName = "Vida das Tropas", description = "+25 HP por nível", maxLevel = 10, baseCost = 120, costMultiplier = 1.5f };

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public UpgradeConfig[] GetAllUpgrades() => new[]
        {
            damageConfig, speedConfig, startCountConfig, multiplyRateConfig, unitHealthConfig
        };

        public int GetLevel(string id) => SaveManager.Instance?.GetUpgradeLevel(id) ?? 1;

        public int GetCost(string id)
        {
            var config = GetConfig(id);
            if (config == null) return 9999;
            return config.GetCostForLevel(GetLevel(id));
        }

        public bool CanUpgrade(string id)
        {
            var config = GetConfig(id);
            if (config == null) return false;
            int level = GetLevel(id);
            if (level >= config.maxLevel) return false;
            int cost = config.GetCostForLevel(level);
            return (SaveManager.Instance?.GetCoins() ?? 0) >= cost;
        }

        public bool TryUpgrade(string id)
        {
            if (!CanUpgrade(id)) return false;
            int cost = GetCost(id);
            if (!Battle.CurrencyManager.Instance.SpendCoins(cost)) return false;
            int newLevel = GetLevel(id) + 1;
            SaveManager.Instance?.SetUpgradeLevel(id, newLevel);
            AudioManager.Instance?.PlayCoin();
            return true;
        }

        private UpgradeConfig GetConfig(string id) => id switch
        {
            "damage" => damageConfig,
            "speed" => speedConfig,
            "startCount" => startCountConfig,
            "multiplyRate" => multiplyRateConfig,
            "unitHealth" => unitHealthConfig,
            _ => null
        };
    }
}
