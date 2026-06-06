using UnityEngine;

namespace PitaRunner.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SkinData", menuName = "PitaRunner/Skin Data")]
    public class SkinData : ScriptableObject
    {
        [Header("Identity")]
        public string skinId;
        public string displayName;
        [TextArea] public string description;

        [Header("Economy")]
        public int cost;
        public bool isFreeDefault;

        [Header("Visual")]
        public GameObject unitPrefab;
        public Sprite thumbnail;
        public Color primaryColor = Color.white;
        public Color secondaryColor = Color.gray;

        [Header("Rarity")]
        public enum Rarity { Common, Rare, Epic, Legendary }
        public Rarity rarity = Rarity.Common;

        public Color RarityColor => rarity switch
        {
            Rarity.Common => Color.gray,
            Rarity.Rare => new Color(0.2f, 0.5f, 1f),
            Rarity.Epic => new Color(0.6f, 0.2f, 0.9f),
            Rarity.Legendary => new Color(1f, 0.7f, 0f),
            _ => Color.white
        };
    }
}
