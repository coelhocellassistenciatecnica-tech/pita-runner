using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PitaRunner.Core;
using PitaRunner.Crowd;

namespace PitaRunner.UI
{
    public class GameUI : MonoBehaviour
    {
        [Header("HUD Elements")]
        [SerializeField] private TextMeshProUGUI unitCountText;
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private Slider progressBar;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private GameObject pauseButton;

        [Header("Panels")]
        [SerializeField] private GameObject hudPanel;
        [SerializeField] private GameObject pausePanel;

        [Header("Unit Count Animation")]
        [SerializeField] private Animator unitCountAnimator;

        private int lastUnitCount = -1;

        private void Start()
        {
            GameManager.OnGameStateChanged += OnStateChanged;
            Battle.CurrencyManager.OnCoinsChanged += UpdateCoinDisplay;
            UpdateCoinDisplay(SaveManager.Instance?.GetCoins() ?? 0);
            if (levelText != null)
                levelText.text = $"FASE {GameManager.Instance?.CurrentLevel ?? 1}";
        }

        private void OnDestroy()
        {
            GameManager.OnGameStateChanged -= OnStateChanged;
            Battle.CurrencyManager.OnCoinsChanged -= UpdateCoinDisplay;
        }

        private void OnStateChanged(GameState state)
        {
            hudPanel?.SetActive(state == GameState.Playing || state == GameState.Battle);
        }

        private void Update()
        {
            if (GameManager.Instance?.CurrentState != GameState.Playing &&
                GameManager.Instance?.CurrentState != GameState.Battle) return;

            UpdateUnitCount();
            UpdateProgress();
        }

        private void UpdateUnitCount()
        {
            int count = CrowdManager.Instance?.UnitCount ?? 0;
            if (count != lastUnitCount)
            {
                lastUnitCount = count;
                if (unitCountText != null)
                    unitCountText.text = count.ToString();
                if (unitCountAnimator != null && count > 0)
                    unitCountAnimator.SetTrigger("Pulse");
            }
        }

        private void UpdateProgress()
        {
            if (progressBar != null && GameManager.Instance != null)
                progressBar.value = GameManager.Instance.LevelProgress;
        }

        private void UpdateCoinDisplay(int coins)
        {
            if (coinText != null) coinText.text = coins.ToString("N0");
        }

        public void OnPausePressed()
        {
            AudioManager.Instance?.PlayButtonClick();
            GameManager.Instance?.PauseGame();
            pausePanel?.SetActive(true);
        }

        public void OnResumePressed()
        {
            AudioManager.Instance?.PlayButtonClick();
            GameManager.Instance?.ResumeGame();
            pausePanel?.SetActive(false);
        }

        public void OnQuitToMenuPressed()
        {
            AudioManager.Instance?.PlayButtonClick();
            Time.timeScale = 1f;
            GameManager.Instance?.GoToMainMenu();
        }
    }
}
