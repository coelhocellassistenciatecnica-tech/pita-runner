using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace PitaRunner.Core
{
    public enum GameState
    {
        MainMenu,
        Playing,
        Battle,
        Victory,
        Defeat,
        Paused
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.MainMenu;

        [Header("Level Settings")]
        [SerializeField] private int currentLevel = 1;
        [SerializeField] private float levelProgressNormalized = 0f;

        public static event Action<GameState> OnGameStateChanged;
        public static event Action<int> OnLevelChanged;
        public static event Action OnVictory;
        public static event Action OnDefeat;

        public GameState CurrentState => currentState;
        public int CurrentLevel => currentLevel;
        public float LevelProgress => levelProgressNormalized;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            currentLevel = SaveManager.Instance != null ? SaveManager.Instance.GetCurrentLevel() : 1;
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
        }

        public void StartGame()
        {
            SetState(GameState.Playing);
            SceneManager.LoadScene("GameScene");
        }

        public void StartBattle()
        {
            SetState(GameState.Battle);
        }

        public void TriggerVictory()
        {
            if (currentState == GameState.Victory) return;
            SetState(GameState.Victory);
            currentLevel++;
            SaveManager.Instance?.SetCurrentLevel(currentLevel);
            OnVictory?.Invoke();
        }

        public void TriggerDefeat()
        {
            if (currentState == GameState.Defeat) return;
            SetState(GameState.Defeat);
            OnDefeat?.Invoke();
        }

        public void RestartLevel()
        {
            SetState(GameState.Playing);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void GoToMainMenu()
        {
            SetState(GameState.MainMenu);
            SceneManager.LoadScene("MainMenu");
        }

        public void SetLevelProgress(float progress)
        {
            levelProgressNormalized = Mathf.Clamp01(progress);
        }

        public void PauseGame()
        {
            if (currentState != GameState.Playing) return;
            SetState(GameState.Paused);
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            if (currentState != GameState.Paused) return;
            SetState(GameState.Playing);
            Time.timeScale = 1f;
        }

        private void SetState(GameState newState)
        {
            currentState = newState;
            OnGameStateChanged?.Invoke(newState);
        }
    }
}
