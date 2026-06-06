using UnityEngine;
using System.Collections.Generic;
using PitaRunner.Core;
using PitaRunner.Portals;

namespace PitaRunner.Level
{
    public class ProceduralLevelGenerator : MonoBehaviour
    {
        [Header("Level Layout")]
        [SerializeField] private float levelLength = 200f;
        [SerializeField] private float trackWidth = 8f;
        [SerializeField] private float tileSize = 10f;

        [Header("Ground")]
        [SerializeField] private GameObject groundTilePrefab;
        [SerializeField] private GameObject[] decorationPrefabs;

        [Header("Portals")]
        [SerializeField] private Portal portalPrefab;
        [SerializeField] private int minPortalPairs = 3;
        [SerializeField] private int maxPortalPairs = 6;
        [SerializeField] private float portalStartZ = 20f;

        [Header("Obstacles")]
        [SerializeField] private GameObject[] obstaclePrefabs;
        [SerializeField] private int minObstacles = 5;
        [SerializeField] private int maxObstacles = 12;
        [SerializeField] private float obstacleStartZ = 15f;

        [Header("Difficulty Scaling")]
        [SerializeField] private float negativePortalChance = 0.2f;
        [SerializeField] private float obstacleScalingPerLevel = 0.1f;

        private List<GameObject> spawnedObjects = new List<GameObject>();

        private void Start()
        {
            GenerateLevel();
        }

        public void GenerateLevel()
        {
            ClearLevel();
            int level = GameManager.Instance?.CurrentLevel ?? 1;

            GenerateGround();
            GeneratePortals(level);
            GenerateObstacles(level);
            GenerateDecorations();
        }

        private void GenerateGround()
        {
            if (groundTilePrefab == null) return;
            int tileCount = Mathf.CeilToInt(levelLength / tileSize) + 2;
            for (int i = 0; i < tileCount; i++)
            {
                Vector3 pos = new Vector3(0f, -0.5f, i * tileSize);
                var tile = Instantiate(groundTilePrefab, pos, Quaternion.identity, transform);
                tile.transform.localScale = new Vector3(trackWidth, 1f, tileSize);
                spawnedObjects.Add(tile);
            }
        }

        private void GeneratePortals(int level)
        {
            if (portalPrefab == null) return;
            int pairCount = Random.Range(minPortalPairs, maxPortalPairs + 1);
            float spacing = (levelLength - portalStartZ - 20f) / pairCount;
            float negChance = negativePortalChance + (level - 1) * 0.02f;

            for (int i = 0; i < pairCount; i++)
            {
                float z = portalStartZ + i * spacing + Random.Range(-2f, 2f);
                SpawnPortalPair(z, level, negChance);
            }
        }

        private void SpawnPortalPair(float z, int level, float negativeChance)
        {
            float xLeft = -trackWidth * 0.25f;
            float xRight = trackWidth * 0.25f;

            bool leftIsPositive = Random.value > negativeChance;
            bool rightIsPositive = !leftIsPositive || Random.value > 0.7f;

            var leftPortal = Instantiate(portalPrefab, new Vector3(xLeft, 0.5f, z), Quaternion.identity, transform);
            var rightPortal = Instantiate(portalPrefab, new Vector3(xRight, 0.5f, z), Quaternion.identity, transform);

            ConfigurePortal(leftPortal, level, leftIsPositive);
            ConfigurePortal(rightPortal, level, rightIsPositive);

            spawnedObjects.Add(leftPortal.gameObject);
            spawnedObjects.Add(rightPortal.gameObject);
        }

        private void ConfigurePortal(Portal portal, int level, bool positive)
        {
            float roll = Random.value;
            PortalType type;
            float multVal = 2f;
            int addVal = 10;

            if (positive)
            {
                if (roll < 0.5f)
                {
                    type = PortalType.Multiply;
                    multVal = Random.value < 0.5f ? 2f : (level >= 3 ? 3f : 2f);
                }
                else
                {
                    type = PortalType.Add;
                    addVal = GetRandomAddValue(level);
                }
            }
            else
            {
                type = PortalType.Add;
                addVal = GetRandomNegativeValue(level);
            }

            portal.Configure(type, multVal, addVal, positive);
        }

        private int GetRandomAddValue(int level)
        {
            int[] values = { 10, 25, 50, 100 };
            return values[Mathf.Min(Random.Range(0, Mathf.Min(level, values.Length)), values.Length - 1)];
        }

        private int GetRandomNegativeValue(int level)
        {
            return Random.Range(5, 10 + level * 2);
        }

        private void GenerateObstacles(int level)
        {
            if (obstaclePrefabs == null || obstaclePrefabs.Length == 0) return;
            int count = Mathf.RoundToInt(Random.Range(minObstacles, maxObstacles) * (1f + (level - 1) * obstacleScalingPerLevel));
            float spacing = (levelLength - obstacleStartZ - 20f) / count;

            for (int i = 0; i < count; i++)
            {
                float z = obstacleStartZ + i * spacing + Random.Range(-1f, 1f);
                float x = Random.Range(-trackWidth * 0.4f, trackWidth * 0.4f);
                var prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
                var obs = Instantiate(prefab, new Vector3(x, 0f, z), Quaternion.identity, transform);
                spawnedObjects.Add(obs);
            }
        }

        private void GenerateDecorations()
        {
            if (decorationPrefabs == null || decorationPrefabs.Length == 0) return;
            int count = 20;
            for (int i = 0; i < count; i++)
            {
                float z = Random.Range(0f, levelLength);
                float x = Random.Range(-trackWidth, trackWidth) * (Random.value > 0.5f ? 1.2f : -1.2f);
                var prefab = decorationPrefabs[Random.Range(0, decorationPrefabs.Length)];
                var deco = Instantiate(prefab, new Vector3(x, 0f, z), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), transform);
                spawnedObjects.Add(deco);
            }
        }

        public void ClearLevel()
        {
            foreach (var obj in spawnedObjects)
                if (obj != null) Destroy(obj);
            spawnedObjects.Clear();
        }
    }
}
