using UnityEngine;

namespace PitaRunner.ScriptableObjects
{
    [CreateAssetMenu(fileName = "BossData", menuName = "PitaRunner/Boss Data")]
    public class BossData : ScriptableObject
    {
        [Header("Identity")]
        public string bossName;
        public Battle.BossType bossType;
        [TextArea] public string description;
        public Sprite portrait;

        [Header("Stats")]
        public float maxHealth = 2000f;
        public float moveSpeed = 2f;
        public float attackDamage = 25f;
        public float attackCooldown = 2f;
        public float defense = 10f;

        [Header("Special Attack")]
        public float specialCooldown = 8f;
        public int specialDamageUnits = 10;
        public float specialRadius = 4f;
        [TextArea] public string specialAttackDescription;

        [Header("Reward")]
        public int bonusCoins = 300;
        public string unlockSkinId;

        [Header("Phases")]
        public float[] healthPhaseThresholds;
        public string[] phaseNames;
    }
}
