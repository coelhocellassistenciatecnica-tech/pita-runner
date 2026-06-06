using UnityEngine;

namespace PitaRunner.ScriptableObjects
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "PitaRunner/Level Data")]
    public class LevelData : ScriptableObject
    {
        [Header("Level Info")]
        public int levelNumber;
        public string levelName;
        public float levelLength = 200f;

        [Header("Difficulty")]
        public int enemyCount = 20;
        public float enemyHealth = 50f;
        public float enemyDamage = 8f;
        public float baseHealth = 500f;
        public float baseDefense = 0f;

        [Header("Portal Settings")]
        public int portalPairCount = 4;
        [Range(0f, 1f)] public float negativePortalChance = 0.2f;

        [Header("Obstacles")]
        public int obstacleCount = 8;
        [Range(0f, 1f)] public float obstacleDensity = 0.5f;

        [Header("Boss")]
        public bool hasBoss = false;
        public ScriptableObjects.BossData bossData;

        [Header("Rewards")]
        public int victoryReward = 100;
        public int perfectBonusReward = 50;
    }
}
