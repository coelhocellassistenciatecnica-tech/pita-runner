using UnityEngine;
using System.Collections.Generic;

namespace PitaRunner.Effects
{
    public class ParticleManager : MonoBehaviour
    {
        public static ParticleManager Instance { get; private set; }

        [Header("Effect Prefabs")]
        [SerializeField] private ParticleSystem hitEffectPrefab;
        [SerializeField] private ParticleSystem deathEffectPrefab;
        [SerializeField] private ParticleSystem multiplyEffectPrefab;
        [SerializeField] private ParticleSystem explosionPrefab;
        [SerializeField] private ParticleSystem victoryEffectPrefab;

        [Header("Pool Settings")]
        [SerializeField] private int poolSize = 20;

        private Dictionary<ParticleSystem, Queue<ParticleSystem>> pools = new();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            InitializePools();
        }

        private void InitializePools()
        {
            PrewarmPool(hitEffectPrefab, poolSize);
            PrewarmPool(deathEffectPrefab, 10);
            PrewarmPool(multiplyEffectPrefab, 5);
            PrewarmPool(explosionPrefab, 8);
        }

        private void PrewarmPool(ParticleSystem prefab, int count)
        {
            if (prefab == null) return;
            var queue = new Queue<ParticleSystem>();
            for (int i = 0; i < count; i++)
            {
                var ps = Instantiate(prefab, transform);
                ps.gameObject.SetActive(false);
                queue.Enqueue(ps);
            }
            pools[prefab] = queue;
        }

        private ParticleSystem GetFromPool(ParticleSystem prefab)
        {
            if (prefab == null) return null;
            if (pools.TryGetValue(prefab, out var queue) && queue.Count > 0)
            {
                var ps = queue.Dequeue();
                queue.Enqueue(ps);
                return ps;
            }
            return Instantiate(prefab, transform);
        }

        private void Play(ParticleSystem prefab, Vector3 position, float scale = 1f)
        {
            var ps = GetFromPool(prefab);
            if (ps == null) return;
            ps.transform.position = position;
            ps.transform.localScale = Vector3.one * scale;
            ps.gameObject.SetActive(true);
            ps.Stop();
            ps.Play();
        }

        public void SpawnHitEffect(Vector3 pos) => Play(hitEffectPrefab, pos, 0.5f);
        public void SpawnDeathEffect(Vector3 pos) => Play(deathEffectPrefab, pos, 0.8f);
        public void SpawnMultiplyEffect(Vector3 pos) => Play(multiplyEffectPrefab, pos, 1.5f);
        public void SpawnExplosion(Vector3 pos) => Play(explosionPrefab, pos, 1.2f);

        public void SpawnVictoryEffect(Vector3 pos)
        {
            if (victoryEffectPrefab != null)
                Instantiate(victoryEffectPrefab, pos, Quaternion.identity);
        }
    }
}
