using UnityEngine;
using TMPro;
using PitaRunner.Crowd;

namespace PitaRunner.Portals
{
    public enum PortalType { Multiply, Add }

    public class Portal : MonoBehaviour
    {
        [Header("Portal Settings")]
        [SerializeField] private PortalType portalType = PortalType.Multiply;
        [SerializeField] private float multiplierValue = 2f;
        [SerializeField] private int addValue = 10;
        [SerializeField] private bool isPositive = true;

        [Header("Visual")]
        [SerializeField] private TextMeshPro labelText;
        [SerializeField] private MeshRenderer portalRenderer;
        [SerializeField] private ParticleSystem portalParticles;
        [SerializeField] private Color positiveColor = new Color(0.2f, 0.9f, 0.3f);
        [SerializeField] private Color negativeColor = new Color(0.9f, 0.2f, 0.2f);

        [Header("Rotation")]
        [SerializeField] private float rotationSpeed = 45f;

        private bool triggered = false;

        private void Start()
        {
            SetupVisuals();
        }

        private void Update()
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }

        private void SetupVisuals()
        {
            Color color = isPositive ? positiveColor : negativeColor;

            if (portalRenderer != null)
            {
                var mat = portalRenderer.material;
                mat.color = color;
                mat.SetColor("_EmissionColor", color * 2f);
            }

            if (portalParticles != null)
            {
                var main = portalParticles.main;
                main.startColor = color;
            }

            UpdateLabel();
        }

        private void UpdateLabel()
        {
            if (labelText == null) return;
            string prefix = isPositive ? "+" : "-";
            if (portalType == PortalType.Multiply)
            {
                prefix = isPositive ? "x" : "/";
                labelText.text = $"{prefix}{multiplierValue}";
            }
            else
            {
                labelText.text = $"{prefix}{addValue}";
            }
            labelText.color = isPositive ? Color.white : Color.red;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (triggered) return;
            if (!other.CompareTag("Player") && !other.CompareTag("Unit")) return;

            triggered = true;
            ApplyEffect();
            StartCoroutine(ResetAfterDelay(0.5f));
        }

        private void ApplyEffect()
        {
            var cm = CrowdManager.Instance;
            if (cm == null) return;

            Core.AudioManager.Instance?.PlayPortalPass();

            if (isPositive)
            {
                if (portalType == PortalType.Multiply)
                    cm.MultiplyUnits(multiplierValue);
                else
                    cm.AddUnits(addValue);
            }
            else
            {
                if (portalType == PortalType.Multiply)
                {
                    int toRemove = Mathf.RoundToInt(cm.UnitCount * (1f - 1f / multiplierValue));
                    cm.RemoveUnits(toRemove);
                }
                else
                {
                    cm.RemoveUnits(addValue);
                }
            }
        }

        private System.Collections.IEnumerator ResetAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            triggered = false;
        }

        public void Configure(PortalType type, float multVal, int addVal, bool positive)
        {
            portalType = type;
            multiplierValue = multVal;
            addValue = addVal;
            isPositive = positive;
            SetupVisuals();
        }
    }
}
