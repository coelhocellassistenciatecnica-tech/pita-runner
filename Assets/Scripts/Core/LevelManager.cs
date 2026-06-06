using UnityEngine;
using PitaRunner.Core;

namespace PitaRunner.Core
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        [Header("Level End Zone")]
        [SerializeField] private GameObject battleArena;
        [SerializeField] private Transform playerStartPosition;
        [SerializeField] private Transform enemyStartPosition;

        [Header("End Zone Trigger")]
        [SerializeField] private float levelEndZ = 200f;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            GameManager.OnGameStateChanged += OnStateChanged;
            if (battleArena != null) battleArena.SetActive(false);
        }

        private void OnDestroy() => GameManager.OnGameStateChanged -= OnStateChanged;

        private void OnStateChanged(GameState state)
        {
            if (state == GameState.Battle)
            {
                if (battleArena != null) battleArena.SetActive(true);
            }
        }

        public float LevelEndZ => levelEndZ;
        public Transform PlayerStart => playerStartPosition;
        public Transform EnemyStart => enemyStartPosition;
    }
}
