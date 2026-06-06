using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace PitaRunner.Effects
{
    public class DamageNumber : MonoBehaviour
    {
        [SerializeField] private TextMeshPro text;
        [SerializeField] private float floatSpeed = 2f;
        [SerializeField] private float fadeDuration = 0.8f;
        [SerializeField] private float scalePunch = 1.4f;

        private Color positiveColor = new Color(1f, 0.9f, 0.1f);
        private Color negativeColor = new Color(1f, 0.3f, 0.3f);

        public void Show(int value, bool isPlayerDamage = true)
        {
            gameObject.SetActive(true);
            if (text == null) text = GetComponentInChildren<TextMeshPro>();
            text.text = $"-{value}";
            text.color = isPlayerDamage ? negativeColor : positiveColor;
            transform.localScale = Vector3.one * scalePunch;
            StopAllCoroutines();
            StartCoroutine(AnimateAndReturn());
        }

        private IEnumerator AnimateAndReturn()
        {
            float elapsed = 0f;
            Vector3 startPos = transform.position;
            Color startColor = text.color;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;
                transform.position = startPos + Vector3.up * (floatSpeed * t);
                transform.localScale = Vector3.Lerp(Vector3.one * scalePunch, Vector3.one, t * 2f);
                text.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);
                yield return null;
            }
            gameObject.SetActive(false);
            DamageNumberPool.Instance?.Return(this);
        }
    }

    public class DamageNumberPool : MonoBehaviour
    {
        public static DamageNumberPool Instance { get; private set; }

        [SerializeField] private DamageNumber prefab;
        [SerializeField] private int poolSize = 30;

        private Queue<DamageNumber> pool = new Queue<DamageNumber>();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            for (int i = 0; i < poolSize; i++)
            {
                var dn = Instantiate(prefab, transform);
                dn.gameObject.SetActive(false);
                pool.Enqueue(dn);
            }
        }

        public void Show(Vector3 worldPos, int value, bool isPlayerDamage = true)
        {
            DamageNumber dn;
            if (pool.Count > 0) dn = pool.Dequeue();
            else dn = Instantiate(prefab, transform);
            dn.transform.position = worldPos;
            dn.gameObject.SetActive(true);
            dn.Show(value, isPlayerDamage);
        }

        public void Return(DamageNumber dn)
        {
            dn.gameObject.SetActive(false);
            pool.Enqueue(dn);
        }
    }
}
