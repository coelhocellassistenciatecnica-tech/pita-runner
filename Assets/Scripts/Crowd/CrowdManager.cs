using UnityEngine;
using System.Collections.Generic;
using PitaRunner.Core;

namespace PitaRunner.Crowd
{
    public class CrowdManager : MonoBehaviour
    {
        public static CrowdManager Instance { get; private set; }

        [Header("Unit Settings")]
        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private int initialUnitCount = 10;
        [SerializeField] private int poolSize = 1200;

        [Header("Formation")]
        [SerializeField] private float unitSpacing = 0.8f;
        [SerializeField] private float formationUpdateRate = 0.05f;
        [SerializeField] private float followSpeed = 12f;

        [Header("Stats (from Upgrades)")]
        [SerializeField] private float unitHealth = 100f;
        [SerializeField] private float unitDamage = 10f;

        private List<Unit> activeUnits = new List<Unit>();
        private Queue<Unit> unitPool = new Queue<Unit>();
        private Transform playerTransform;
        private float formationTimer = 0f;

        public int UnitCount => activeUnits.Count;
        public List<Unit> ActiveUnits => activeUnits;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            playerTransform = Player.PlayerController.Instance?.transform;
            InitializePool();
            ApplyUpgrades();
            SpawnInitialUnits();
            GameManager.OnGameStateChanged += OnStateChanged;
        }

        private void OnDestroy() => GameManager.OnGameStateChanged -= OnStateChanged;

        private void OnStateChanged(GameState state)
        {
            if (state == GameState.Battle)
                StartBattle();
        }

        private void InitializePool()
        {
            for (int i = 0; i < poolSize; i++)
            {
                var go = Instantiate(unitPrefab, Vector3.zero, Quaternion.identity);
                go.SetActive(false);
                var unit = go.GetComponent<Unit>();
                unitPool.Enqueue(unit);
            }
        }

        private void ApplyUpgrades()
        {
            if (SaveManager.Instance == null) return;
            int healthLevel = SaveManager.Instance.GetUpgradeLevel("unitHealth");
            int damageLevel = SaveManager.Instance.GetUpgradeLevel("damage");
            int startLevel = SaveManager.Instance.GetUpgradeLevel("startCount");
            unitHealth = 80f + (healthLevel - 1) * 25f;
            unitDamage = 8f + (damageLevel - 1) * 5f;
            initialUnitCount = 10 + (startLevel - 1) * 5;
        }

        private void SpawnInitialUnits()
        {
            for (int i = 0; i < initialUnitCount; i++)
                SpawnUnit();
        }

        public Unit SpawnUnit()
        {
            Unit unit;
            if (unitPool.Count > 0)
            {
                unit = unitPool.Dequeue();
            }
            else
            {
                var go = Instantiate(unitPrefab);
                unit = go.GetComponent<Unit>();
            }
            unit.Initialize(unitHealth, unitDamage);
            activeUnits.Add(unit);
            SaveManager.Instance?.UpdateHighestUnitCount(activeUnits.Count);
            return unit;
        }

        public void ReturnToPool(Unit unit)
        {
            unit.gameObject.SetActive(false);
            unitPool.Enqueue(unit);
        }

        public void AddUnits(int count)
        {
            for (int i = 0; i < count; i++)
                SpawnUnit();
            AudioManager.Instance?.PlayMultiply();
            Effects.ParticleManager.Instance?.SpawnMultiplyEffect(playerTransform != null ? playerTransform.position : Vector3.zero);
        }

        public void MultiplyUnits(float multiplier)
        {
            int current = activeUnits.Count;
            int toAdd = Mathf.RoundToInt(current * (multiplier - 1f));
            toAdd = Mathf.Min(toAdd, poolSize - current);
            AddUnits(toAdd);
        }

        public void RemoveUnits(int count)
        {
            int toRemove = Mathf.Min(count, activeUnits.Count);
            for (int i = 0; i < toRemove; i++)
            {
                if (activeUnits.Count == 0) break;
                var unit = activeUnits[activeUnits.Count - 1];
                activeUnits.RemoveAt(activeUnits.Count - 1);
                Effects.ParticleManager.Instance?.SpawnDeathEffect(unit.transform.position);
                ReturnToPool(unit);
            }
            if (activeUnits.Count == 0)
                GameManager.Instance?.TriggerDefeat();
        }

        public void OnUnitDied(Unit unit)
        {
            activeUnits.Remove(unit);
            unitPool.Enqueue(unit);
            if (activeUnits.Count == 0)
                GameManager.Instance?.TriggerDefeat();
        }

        private void Update()
        {
            if (GameManager.Instance?.CurrentState != GameState.Playing) return;
            if (playerTransform == null) return;

            formationTimer += Time.deltaTime;
            if (formationTimer >= formationUpdateRate)
            {
                formationTimer = 0f;
                UpdateFormation();
            }
        }

        private void UpdateFormation()
        {
            int count = activeUnits.Count;
            if (count == 0) return;

            int cols = Mathf.CeilToInt(Mathf.Sqrt(count));
            int rows = Mathf.CeilToInt((float)count / cols);

            float startX = -(cols - 1) * unitSpacing * 0.5f;
            float startZ = -(rows - 1) * unitSpacing * 0.5f;

            for (int i = 0; i < count; i++)
            {
                int col = i % cols;
                int row = i / cols;
                Vector3 localOffset = new Vector3(startX + col * unitSpacing, 0f, startZ + row * unitSpacing);
                Vector3 targetPos = playerTransform.position + localOffset;

                activeUnits[i].transform.position = Vector3.Lerp(
                    activeUnits[i].transform.position,
                    targetPos,
                    followSpeed * Time.deltaTime
                );
            }
        }

        private void StartBattle()
        {
            foreach (var unit in activeUnits)
                unit.EnterBattle();
        }

        public Unit FindNearestEnemy(Vector3 position)
        {
            var enemies = BattleManager.Instance?.EnemyUnits;
            if (enemies == null || enemies.Count == 0) return null;
            Unit nearest = null;
            float minDist = float.MaxValue;
            foreach (var e in enemies)
            {
                if (e == null || !e.IsAlive) continue;
                float d = Vector3.Distance(position, e.transform.position);
                if (d < minDist) { minDist = d; nearest = e; }
            }
            return nearest;
        }
    }
}
