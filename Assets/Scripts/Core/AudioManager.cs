using UnityEngine;
using System.Collections.Generic;

namespace PitaRunner.Core
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource sfxSource2;
        [SerializeField] private AudioSource sfxSource3;

        [Header("Music Clips")]
        [SerializeField] private AudioClip mainMenuMusic;
        [SerializeField] private AudioClip gameplayMusic;
        [SerializeField] private AudioClip battleMusic;
        [SerializeField] private AudioClip victoryMusic;
        [SerializeField] private AudioClip defeatMusic;

        [Header("SFX Clips")]
        [SerializeField] private AudioClip portalPassClip;
        [SerializeField] private AudioClip multiplyClip;
        [SerializeField] private AudioClip unitDieClip;
        [SerializeField] private AudioClip attackClip;
        [SerializeField] private AudioClip victoryClip;
        [SerializeField] private AudioClip defeatClip;
        [SerializeField] private AudioClip coinClip;
        [SerializeField] private AudioClip buttonClickClip;
        [SerializeField] private AudioClip bossAttackClip;
        [SerializeField] private AudioClip explosionClip;

        [Header("Settings")]
        [Range(0f, 1f)] [SerializeField] private float musicVolume = 0.6f;
        [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;

        private bool isMuted = false;
        private Queue<AudioSource> sfxPool = new Queue<AudioSource>();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            sfxPool.Enqueue(sfxSource);
            sfxPool.Enqueue(sfxSource2);
            sfxPool.Enqueue(sfxSource3);
        }

        private void Start()
        {
            GameManager.OnGameStateChanged += OnStateChanged;
            PlayMusic(mainMenuMusic);
        }

        private void OnDestroy() => GameManager.OnGameStateChanged -= OnStateChanged;

        private void OnStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Playing: PlayMusic(gameplayMusic); break;
                case GameState.Battle: PlayMusic(battleMusic); break;
                case GameState.Victory: PlayMusic(victoryMusic); PlaySFX(victoryClip); break;
                case GameState.Defeat: PlayMusic(defeatMusic); PlaySFX(defeatClip); break;
                case GameState.MainMenu: PlayMusic(mainMenuMusic); break;
            }
        }

        public void PlayMusic(AudioClip clip)
        {
            if (clip == null || isMuted) return;
            if (musicSource.clip == clip && musicSource.isPlaying) return;
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }

        public void PlaySFX(AudioClip clip)
        {
            if (clip == null || isMuted) return;
            if (sfxPool.Count > 0)
            {
                var src = sfxPool.Dequeue();
                src.PlayOneShot(clip, sfxVolume);
                sfxPool.Enqueue(src);
            }
            else
            {
                sfxSource.PlayOneShot(clip, sfxVolume);
            }
        }

        public void PlayPortalPass() => PlaySFX(portalPassClip);
        public void PlayMultiply() => PlaySFX(multiplyClip);
        public void PlayUnitDie() => PlaySFX(unitDieClip);
        public void PlayAttack() => PlaySFX(attackClip);
        public void PlayCoin() => PlaySFX(coinClip);
        public void PlayButtonClick() => PlaySFX(buttonClickClip);
        public void PlayBossAttack() => PlaySFX(bossAttackClip);
        public void PlayExplosion() => PlaySFX(explosionClip);

        public void SetMusicVolume(float v) { musicVolume = v; musicSource.volume = v; }
        public void SetSFXVolume(float v) { sfxVolume = v; }
        public void ToggleMute() { isMuted = !isMuted; musicSource.mute = isMuted; }
        public bool IsMuted => isMuted;
    }
}
