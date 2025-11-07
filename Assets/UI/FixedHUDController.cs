using UnityEngine;
using UnityEngine.UI;
using Core;
using Characters;
using System.Collections;

namespace UI
{
    public class FixedHUDController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Slider healthBar;
        [SerializeField] private Text healthText;
        [SerializeField] private Slider expBar;
        [SerializeField] private Text levelText;
        [SerializeField] private Text expText;

        private PlayerStats playerStats;
        private bool eventManagerReady = false;

        private void Start()
        {
            Debug.Log("🔄 FixedHUDController starting...");

            // 1. СНАЧАЛА ИЩЕМ PlayerStats
            playerStats = FindFirstObjectByType<PlayerStats>();
            if (playerStats == null)
            {
                Debug.LogError("❌ PlayerStats not found!");
                return;
            }
            Debug.Log("✅ PlayerStats found");

            // 2. ЖДЕМ EventManager
            StartCoroutine(WaitForEventManager());
        }

        private System.Collections.IEnumerator WaitForEventManager()
        {
            Debug.Log("⏳ Waiting for EventManager...");

            // Ждем пока EventManager проинициализируется
            while (EventManager.Instance == null)
            {
                yield return new WaitForSeconds(0.1f);
            }

            eventManagerReady = true;
            Debug.Log("✅ EventManager is ready!");

            // ПОДПИСКА НА СОБЫТИЯ (ИСПРАВЛЕННАЯ)
            EventManager.Instance.StartListening(GameEventType.PlayerHealthChanged, OnHealthChanged);
            EventManager.Instance.StartListening(GameEventType.ExperienceChanged, OnExperienceChanged); // ← ИСПРАВЛЕНО!
            EventManager.Instance.StartListening(GameEventType.PlayerLevelUp, OnLevelUp);
            // EventManager.Instance.StartListening(GameEventType.EnemyDied, OnEnemyDied); // ← УБЕРИТЕ ЭТУ СТРОКУ!

            // ПЕРВОНАЧАЛЬНОЕ ОБНОВЛЕНИЕ
            UpdateAllDisplays();
        }

        private void Update()
        {
            // ВРЕМЕННО: принудительное обновление если EventManager не готов
            if (!eventManagerReady && playerStats != null)
            {
                UpdateAllDisplays();
            }
        }

        private void UpdateAllDisplays()
        {
            UpdateHealthDisplay();
            UpdateExperienceDisplay();
            UpdateLevelDisplay();
        }

        private void OnHealthChanged(object healthData)
        {
            Debug.Log($"❤️ Health changed: {healthData}");
            UpdateHealthDisplay();
        }

        private void UpdateHealthDisplay()
        {
            if (playerStats == null) return;

            // ЗАЩИТА ОТ NULL
            if (healthBar != null)
            {
                float healthPercent = (float)playerStats.CurrentHealth / playerStats.MaxHealth;
                healthBar.value = healthPercent;
            }

            if (healthText != null)
                healthText.text = $"{playerStats.CurrentHealth}/{playerStats.MaxHealth}";
        }

        private void OnExperienceChanged(object expData) // ← ИСПРАВЛЕННОЕ ИМЯ!
        {
            // ЗАЩИТА ОТ МУСОРНЫХ ДАННЫХ
            if (expData is int currentExp && currentExp >= 0)
            {
                Debug.Log($"⭐ Experience changed: {currentExp}");
                UpdateExperienceDisplay();
            }
            else
            {
                Debug.LogError($"❌ Invalid experience data in HUD: {expData}");
            }
        }

        private void UpdateExperienceDisplay()
        {
            if (playerStats == null) return;

            if (expBar != null)
            {
                float expPercent = (float)playerStats.Experience / playerStats.ExperienceToNextLevel;
                expBar.value = expPercent;
            }

            if (expText != null)
                expText.text = $"{playerStats.Experience}/{playerStats.ExperienceToNextLevel} XP";
        }

        private void OnLevelUp(object levelData)
        {
            Debug.Log($"🎯 Level up: {levelData}");
            UpdateLevelDisplay();
        }

        private void UpdateLevelDisplay()
        {
            if (playerStats != null && levelText != null)
                levelText.text = $"Level {playerStats.Level}";
        }

        // УБЕРИТЕ ЭТОТ МЕТОД - он не нужен!
        // private void OnEnemyDied(object enemy) { }
    }
}