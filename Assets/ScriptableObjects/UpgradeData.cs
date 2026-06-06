using UnityEngine;

namespace PitaRunner.ScriptableObjects
{
    [CreateAssetMenu(fileName = "UpgradeData", menuName = "PitaRunner/Upgrade Data")]
    public class UpgradeData : ScriptableObject
    {
        [Header("Identity")]
        public string upgradeId;
        public string displayName;
        [TextArea] public string description;
        public Sprite icon;

        [Header("Levels")]
        public int maxLevel = 10;
        public int baseCost = 100;
        public float costMultiplier = 1.5f;

        [Header("Values per Level")]
        public float[] valuePerLevel;

        public int GetCostForLevel(int level)
            => Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplier, level - 1));

        public float GetValueForLevel(int level)
        {
            if (valuePerLevel == null || valuePerLevel.Length == 0) return 0f;
            int idx = Mathf.Clamp(level - 1, 0, valuePerLevel.Length - 1);
            return valuePerLevel[idx];
        }
    }
}
