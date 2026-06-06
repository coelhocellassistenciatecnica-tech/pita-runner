using UnityEngine;
using TMPro;
using PitaRunner.Core;

namespace PitaRunner.UI
{
    public class DefeatUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject defeatPanel;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI tipText;
        [SerializeField] private Animator panelAnimator;

        private readonly string[] tips = new string[]
        {
            "Dica: Escolha portais x2 sempre que possível!",
            "Dica: Evite as serras giratórias — elas eliminam várias unidades!",
            "Dica: Portais +100 aparecem em fases avançadas.",
            "Dica: Faça upgrades de Quantidade Inicial para começar mais forte.",
            "Dica: Acumule unidades antes da batalha final!",
            "Dica: Toque e arraste para desviar dos obstáculos.",
        };

        private void Start()
        {
            defeatPanel?.SetActive(false);
            GameManager.OnDefeat += OnDefeat;
        }

        private void OnDestroy() => GameManager.OnDefeat -= OnDefeat;

        private void OnDefeat()
        {
            defeatPanel?.SetActive(true);
            if (panelAnimator != null) panelAnimator.SetTrigger("Show");

            int level = GameManager.Instance?.CurrentLevel ?? 1;
            if (levelText != null) levelText.text = $"FASE {level}";
            if (tipText != null) tipText.text = tips[Random.Range(0, tips.Length)];

            SaveManager.Instance?.RecordDefeat();
        }

        public void OnRetryPressed()
        {
            AudioManager.Instance?.PlayButtonClick();
            GameManager.Instance?.RestartLevel();
        }

        public void OnMainMenuPressed()
        {
            AudioManager.Instance?.PlayButtonClick();
            GameManager.Instance?.GoToMainMenu();
        }

        public void OnWatchAdToRevive()
        {
            AudioManager.Instance?.PlayButtonClick();
            // Integração com Unity Ads (placeholder)
            Debug.Log("[Monetization] Rewarded Ad solicitado para reviver.");
            RevivePlayer();
        }

        private void RevivePlayer()
        {
            defeatPanel?.SetActive(false);
            CrowdManager.Instance?.AddUnits(15);
            GameManager.Instance?.StartBattle();
        }
    }
}
