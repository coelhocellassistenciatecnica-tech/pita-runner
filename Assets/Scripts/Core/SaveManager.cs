using UnityEngine;
using System;

namespace PitaRunner.Core
{
    [Serializable]
    public class SaveData
    {
        public int currentLevel = 1;
        public int coins = 0;
        public int totalCoinsEarned = 0;
        public string equippedSkin = "Soldier";
        public string[] unlockedSkins = new string[] { "Soldier" };

        // Upgrades
        public int damageLevel = 1;
        public int speedLevel = 1;
        public int startCountLevel = 1;
        public int multiplyRateLevel = 1;
        public int unitHealthLevel = 1;

        // Stats
        public int totalVictories = 0;
        public int totalDefeats = 0;
        public int highestUnitCount = 0;
    }

    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        private const string SAVE_KEY = "PitaRunnerSave";
        private SaveData saveData;

        public SaveData Data => saveData;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }

        private void Load()
        {
            string json = PlayerPrefs.GetString(SAVE_KEY, "");
            if (!string.IsNullOrEmpty(json))
            {
                saveData = JsonUtility.FromJson<SaveData>(json);
            }
            else
            {
                saveData = new SaveData();
            }
        }

        public void Save()
        {
            string json = JsonUtility.ToJson(saveData);
            PlayerPrefs.SetString(SAVE_KEY, json);
            PlayerPrefs.Save();
        }

        public int GetCurrentLevel() => saveData.currentLevel;
        public void SetCurrentLevel(int level) { saveData.currentLevel = level; Save(); }

        public int GetCoins() => saveData.coins;
        public void AddCoins(int amount)
        {
            saveData.coins += amount;
            saveData.totalCoinsEarned += amount;
            Save();
        }
        public bool SpendCoins(int amount)
        {
            if (saveData.coins < amount) return false;
            saveData.coins -= amount;
            Save();
            return true;
        }

        public string GetEquippedSkin() => saveData.equippedSkin;
        public void SetEquippedSkin(string skinId) { saveData.equippedSkin = skinId; Save(); }

        public bool IsSkinUnlocked(string skinId)
        {
            foreach (var s in saveData.unlockedSkins)
                if (s == skinId) return true;
            return false;
        }

        public void UnlockSkin(string skinId)
        {
            var list = new System.Collections.Generic.List<string>(saveData.unlockedSkins);
            if (!list.Contains(skinId))
            {
                list.Add(skinId);
                saveData.unlockedSkins = list.ToArray();
                Save();
            }
        }

        public int GetUpgradeLevel(string upgradeId)
        {
            switch (upgradeId)
            {
                case "damage": return saveData.damageLevel;
                case "speed": return saveData.speedLevel;
                case "startCount": return saveData.startCountLevel;
                case "multiplyRate": return saveData.multiplyRateLevel;
                case "unitHealth": return saveData.unitHealthLevel;
                default: return 1;
            }
        }

        public void SetUpgradeLevel(string upgradeId, int level)
        {
            switch (upgradeId)
            {
                case "damage": saveData.damageLevel = level; break;
                case "speed": saveData.speedLevel = level; break;
                case "startCount": saveData.startCountLevel = level; break;
                case "multiplyRate": saveData.multiplyRateLevel = level; break;
                case "unitHealth": saveData.unitHealthLevel = level; break;
            }
            Save();
        }

        public void RecordVictory() { saveData.totalVictories++; Save(); }
        public void RecordDefeat() { saveData.totalDefeats++; Save(); }
        public void UpdateHighestUnitCount(int count)
        {
            if (count > saveData.highestUnitCount)
            {
                saveData.highestUnitCount = count;
                Save();
            }
        }

        public void DeleteSave()
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
            saveData = new SaveData();
        }
    }
}
