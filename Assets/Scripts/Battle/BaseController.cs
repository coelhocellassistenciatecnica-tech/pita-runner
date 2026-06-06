using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PitaRunner.Core;

namespace PitaRunner.Battle
{
    public class BaseController : MonoBehaviour
    {
        [Header("Base Settings")]
        [SerializeField] private float maxHealth = 500f;
        [SerializeField] private float defense = 0f;
        [SerializeField] private bool isPlayerBase = false;

        [Header("UI")]
        [SerializeField] private Slider healthBar;
        [SerializeField] private TextMeshProUGUI healthText;

        [Header("Visuals")]
        [SerializeField] private GameObject destroyEffect;
        [SerializeField] private Renderer baseRenderer;

        private float currentHealth;
        private bool destroyed = false;

        public bool IsDestroyed => destroyed;
        public float HealthPercent => currentHealth / maxHealth;

        private void Start()
        {
            currentHealth = maxHealth;
            UpdateUI();
        }

        public void TakeDamage(float rawDamage)
        {
            if (destroyed) return;
            float actualDamage = Mathf.Max(1f, rawDamage - defense);
            currentHealth -= actualDamage;
            currentHealth = Mathf.Max(0, currentHealth);
            UpdateUI();
            Effects.DamageNumberPool.Instance?.Show(transform.position + Vector3.up * 2f, (int)actualDamage, isPlayerBase);
            ShakeBase();
            if (currentHealth <= 0) OnDestroyed();
        }

        private void OnDestroyed()
        {
            destroyed = true;
            if (destroyEffect != null) Instantiate(destroyEffect, transform.position, Quaternion.identity);
            Effects.ParticleManager.Instance?.SpawnExplosion(transform.position);
            AudioManager.Instance?.PlayExplosion();

            if (isPlayerBase)
                GameManager.Instance?.TriggerDefeat();
            else
            {
                GameManager.Instance?.TriggerVictory();
                CurrencyManager.Instance?.AddCoinsFromVictory();
            }
        }

        private void UpdateUI()
        {
            if (healthBar != null) healthBar.value = HealthPercent;
            if (healthText != null) healthText.text = $"{Mathf.CeilToInt(currentHealth)}/{Mathf.CeilToInt(maxHealth)}";
        }

        private void ShakeBase()
        {
            StopAllCoroutines();
            StartCoroutine(ShakeCoroutine());
        }

        private System.Collections.IEnumerator ShakeCoroutine()
        {
            Vector3 originalPos = transform.localPosition;
            float elapsed = 0f;
            while (elapsed < 0.2f)
            {
                transform.localPosition = originalPos + Random.insideUnitSphere * 0.1f;
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.localPosition = originalPos;
        }

        public void SetHealth(float hp, float def = 0f)
        {
            maxHealth = hp;
            defense = def;
            currentHealth = hp;
            UpdateUI();
        }
    }
}
