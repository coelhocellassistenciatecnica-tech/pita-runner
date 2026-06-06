using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PitaRunner.Core;
using PitaRunner.Crowd;

namespace PitaRunner.Battle
{
    public class BattleManager : MonoBehaviour
    {
        public static BattleManager Instance { get; private set; }

        [Header("Enemy Setup")]
        [SerializeField] private GameObject enemyUnitPrefab;
        [SerializeField] private BaseController enemyBase;
        [SerializeField] private BaseController playerBase;
        [SerializeField] private int baseEnemyCount = 20;
        [SerializeField] private float enemySpawnDelay = 0.05f;
        [SerializeField] private Transform enemySpawnPoint;

        [Header("Boss")]
        [SerializeField] private BossController bossPrefab;
        [SerializeField] private bool spawnBoss = false;

        private List<EnemyUnit> enemyUnits = new List<EnemyUnit>();
        private BossController activeBoss;
        private bool battleStarted = false;

        public List<EnemyUnit> EnemyUnits => enemyUnits;
        public BaseController EnemyBase => enemyBase;
        public BaseController PlayerBase => playerBase;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            GameManager.OnGameStateChanged += OnStateChanged;
        }

        private void OnDestroy() => GameManager.OnGameStateChanged -= OnStateChanged;

        private void OnStateChanged(GameState state)
        {
            if (state == GameState.Battle && !battleStarted)
            {
                battleStarted = true;
                StartCoroutine(StartBattleSequence());
            }
        }

        private IEnumerator StartBattleSequence()
        {
            int enemyCount = CalculateEnemyCount();
            for (int i = 0; i < enemyCount; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(enemySpawnDelay);
            }

            if (spawnBoss && bossPrefab != null)
            {
                Vector3 bossPos = enemySpawnPoint != null ? enemySpawnPoint.position + Vector3.back * 3f : Vector3.back * 20f;
                activeBoss = Instantiate(bossPrefab, bossPos, Quaternion.identity);
                activeBoss.Initialize(this);
            }

            foreach (var unit in CrowdManager.Instance.ActiveUnits)
                unit.EnterBattle();

            foreach (var enemy in enemyUnits)
                enemy.EnterBattle();
        }

        private int CalculateEnemyCount()
        {
            int level = GameManager.Instance?.CurrentLevel ?? 1;
            return baseEnemyCount + (level - 1) * 5;
        }

        private void SpawnEnemy()
        {
            if (enemyUnitPrefab == null || enemySpawnPoint == null) return;
            Vector3 spawnPos = enemySpawnPoint.position + new Vector3(
                Random.Range(-3f, 3f), 0f, Random.Range(-2f, 2f));
            var go = Instantiate(enemyUnitPrefab, spawnPos, Quaternion.identity);
            var enemy = go.GetComponent<EnemyUnit>();
            if (enemy != null)
            {
                float level = GameManager.Instance?.CurrentLevel ?? 1;
                enemy.Initialize(50f + level * 10f, 8f + level * 2f, this);
                enemyUnits.Add(enemy);
            }
        }

        public void OnEnemyDied(EnemyUnit enemy)
        {
            enemyUnits.Remove(enemy);
            if (enemyUnits.Count == 0 && (activeBoss == null || activeBoss.IsDead))
            {
                CheckBattleEnd();
            }
        }

        public void CheckBattleEnd()
        {
            if (enemyBase != null && enemyBase.IsDestroyed)
            {
                GameManager.Instance?.TriggerVictory();
            }
        }

        public Unit FindNearestPlayerUnit(Vector3 position)
        {
            var units = CrowdManager.Instance?.ActiveUnits;
            if (units == null || units.Count == 0) return null;
            Unit nearest = null;
            float minDist = float.MaxValue;
            foreach (var u in units)
            {
                if (!u.IsAlive) continue;
                float d = Vector3.Distance(position, u.transform.position);
                if (d < minDist) { minDist = d; nearest = u; }
            }
            return nearest;
        }
    }
}
