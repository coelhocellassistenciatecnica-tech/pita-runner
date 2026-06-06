using UnityEngine;

namespace PitaRunner.ScriptableObjects
{
    [CreateAssetMenu(fileName = "PortalData", menuName = "PitaRunner/Portal Data")]
    public class PortalData : ScriptableObject
    {
        [Header("Portal Configuration")]
        public Portals.PortalType portalType = Portals.PortalType.Multiply;
        public float multiplierValue = 2f;
        public int addValue = 10;
        public bool isPositive = true;

        [Header("Visuals")]
        public Color positiveColor = new Color(0.2f, 0.9f, 0.3f);
        public Color negativeColor = new Color(0.9f, 0.2f, 0.2f);
        public float rotationSpeed = 45f;

        [Header("Probability")]
        [Range(0f, 1f)] public float spawnWeight = 1f;

        public string GetLabel()
        {
            string prefix;
            if (portalType == Portals.PortalType.Multiply)
                prefix = isPositive ? "x" : "/";
            else
                prefix = isPositive ? "+" : "-";

            string value = portalType == Portals.PortalType.Multiply
                ? multiplierValue.ToString("F0")
                : addValue.ToString();

            return prefix + value;
        }
    }
}
