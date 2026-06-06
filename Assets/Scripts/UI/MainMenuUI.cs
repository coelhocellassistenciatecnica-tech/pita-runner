using UnityEngine;
using TMPro;
using PitaRunner.Core;

namespace PitaRunner.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject settingsPanel;

        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI versionText;

        [Header("Settings")]
        [SerializeField] private UnityEngine.UI.Slider musicSlider;
        [SerializeField] private UnityEngine.UI.Slider sfxSlider;
        [SerializeField] private UnityEngine.UI.Toggle muteToggle;

        [Header("Logo Animation")]
        [SerializeField] private Animator logoAnimator;

        private void Start()
        {
            mainPanel?.SetActive(true);
            settingsPanel?.SetActive(false);

            int coins = SaveManager.Instance?.GetCoins() ?? 0;
            int level = SaveManager.Instance?.GetCurrentLevel() ?? 1;

            if (coinText != null) coinText.text = coins.ToString("N0");
            if (levelText != null) levelText.text = $"FASE {level}";
            if (versionText != null) versionText.text = $"v1.0.0";

            if (logoAnimator != null) logoAnimator.SetTrigger("Appear");
        }

        public void OnPlayPressed()
        {
            AudioManager.Instance?.PlayButtonClick();
            GameManager.Instance?.StartGame();
        }

        public void OnShopPressed()
        {
            AudioManager.Instance?.PlayButtonClick();
            UnityEngine.SceneManagement.SceneManager.LoadScene("ShopScene");
        }

        public void OnSkinsPressed()
        {
            AudioManager.Instance?.PlayButtonClick();
            UnityEngine.SceneManagement.SceneManager.LoadScene("SkinsScene");
        }

        public void OnSettingsPressed()
        {
            AudioManager.Instance?.PlayButtonClick();
            mainPanel?.SetActive(false);
            settingsPanel?.SetActive(true);
        }

        public void OnSettingsBackPressed()
        {
            AudioManager.Instance?.PlayButtonClick();
            mainPanel?.SetActive(true);
            settingsPanel?.SetActive(false);
        }

        public void OnMusicVolumeChanged(float value)
        {
            AudioManager.Instance?.SetMusicVolume(value);
        }

        public void OnSFXVolumeChanged(float value)
        {
            AudioManager.Instance?.SetSFXVolume(value);
        }

        public void OnMuteToggled(bool muted)
        {
            AudioManager.Instance?.ToggleMute();
        }
    }
}
