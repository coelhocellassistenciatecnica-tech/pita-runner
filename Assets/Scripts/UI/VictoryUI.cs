using UnityEngine;
using TMPro;
using System.Collections;
using PitaRunner.Core;
using PitaRunner.Crowd;

namespace PitaRunner.UI
{
    public class VictoryUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject victoryPanel;
        [SerializeField] private TextMeshProUGUI coinsEarnedText;
        [SerializeField] private TextMeshProUGUI unitsSurvivedText;
        [SerializeField] private TextMeshProUGUI nextLevelText;
        [SerializeField] private Animator panelAnimator;

        [Header("Coin Particles")]
        [SerializeField] private ParticleSystem coinParticles;
        [SerializeField] private ParticleSystem confettiParticles;

        [Header("Reward Display")]
        [SerializeField] private float countUpDuration = 1.5f;

        private void Start()
        {
            victoryPanel?.SetActive(false);
            GameManager.OnVictory += OnVictory;
        }

        private void OnDestroy() => GameManager.OnVictory -= OnVictory;

        private void OnVictory()
        {
            victoryPanel?.SetActive(true);
            if (panelAnimator != null) panelAnimator.SetTrigger("Show");
            if (coinParticles != null) coinParticles.Play();
            if (confettiParticles != null) confettiParticles.Play();

            int reward = Battle.CurrencyManager.Instance?.CalculateVictoryReward() ?? 0;
            int survivors = CrowdManager.Instance?.UnitCount ?? 0;

            StartCoroutine(AnimateReward(reward, survivors));
        }

        private IEnumerator AnimateReward(int targetCoins, int survivors)
        {
            float elapsed = 0f;
            while (elapsed < countUpDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / countUpDuration;
                int displayCoins = Mathf.RoundToInt(Mathf.Lerp(0, targetCoins, t));
                if (coinsEarnedText != null) coinsEarnedText.text = $"+{displayCoins}";
                yield return null;
            }
            if (coinsEarnedText != null) coinsEarnedText.text = $"+{targetCoins}";
            if (unitsSurvivedText != null) unitsSurvivedText.text = $"{survivors} tropas";

            int nextLevel = GameManager.Instance?.CurrentLevel ?? 1;
            if (nextLevelText != null) nextLevelText.text = $"FASE {nextLevel}";
        }

        public void OnNextLevelPressed()
        {
            AudioManager.Instance?.PlayButtonClick();
            GameManager.Instance?.StartGame();
        }

        public void OnMainMenuPressed()
        {
            AudioManager.Instance?.PlayButtonClick();
            GameManager.Instance?.GoToMainMenu();
        }
    }
}
