using UnityEngine;
using System;
using PitaRunner.Core;

namespace PitaRunner.Battle
{
    public class CurrencyManager : MonoBehaviour
    {
        public static CurrencyManager Instance { get; private set; }

        [Header("Rewards")]
        [SerializeField] private int baseVictoryReward = 100;
        [SerializeField] private int baseDestructionReward = 50;
        [SerializeField] private int coinsPerUnit = 2;

        public static event Action<int> OnCoinsChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public int GetCoins() => SaveManager.Instance?.GetCoins() ?? 0;

        public void AddCoins(int amount)
        {
            SaveManager.Instance?.AddCoins(amount);
            OnCoinsChanged?.Invoke(GetCoins());
        }

        public bool SpendCoins(int amount)
        {
            bool success = SaveManager.Instance?.SpendCoins(amount) ?? false;
            if (success) OnCoinsChanged?.Invoke(GetCoins());
            return success;
        }

        public void AddCoinsFromVictory()
        {
            int level = GameManager.Instance?.CurrentLevel ?? 1;
            int unitBonus = (Crowd.CrowdManager.Instance?.UnitCount ?? 0) * coinsPerUnit;
            int reward = baseVictoryReward + (level - 1) * 20 + unitBonus;
            AddCoins(reward);
            SaveManager.Instance?.RecordVictory();
        }

        public int CalculateVictoryReward()
        {
            int level = GameManager.Instance?.CurrentLevel ?? 1;
            int unitBonus = (Crowd.CrowdManager.Instance?.UnitCount ?? 0) * coinsPerUnit;
            return baseVictoryReward + (level - 1) * 20 + unitBonus;
        }
    }
}
