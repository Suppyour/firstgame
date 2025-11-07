using UnityEngine;
using UnityEngine.UI;
using Core;
using Characters;

namespace UI
{
    public class HUDController : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private Slider healthBar;
        [SerializeField] private Text healthText;

        [Header("Experience")]
        [SerializeField] private Slider expBar;
        [SerializeField] private Text levelText;
        [SerializeField] private Text expText;

        private PlayerStats playerStats;

        private void Start()
        {
            Debug.Log("🔍 HUDController checking event subscriptions...");

            // ПРОВЕРКА ПОДПИСКИ
            if (EventManager.Instance != null)
            {
                EventManager.Instance.StartListening(GameEventType.PlayerHealthChanged, OnHealthChanged);
                EventManager.Instance.StartListening(GameEventType.ExperienceGained, OnExperienceGained);
                Debug.Log("✅ Subscribed to events");
            }
            else
            {
                Debug.LogError("❌ EventManager.Instance is NULL!");
            }

            // ПРЯМАЯ ПРОВЕРКА СВЯЗИ
            if (healthBar == null) Debug.LogError("❌ HealthBar not assigned!");
            if (expBar == null) Debug.LogError("❌ ExpBar not assigned!");

            Debug.Log("=== HUD CONTROLLER START ===");

            // ПРОВЕРКА ССЫЛОК НА UI ЭЛЕМЕНТЫ
            if (healthBar == null) Debug.LogError("❌ HealthBar not assigned in Inspector!");
            else Debug.Log("✅ HealthBar reference OK");

            if (expBar == null) Debug.LogError("❌ ExpBar not assigned in Inspector!");
            else Debug.Log("✅ ExpBar reference OK");

            if (healthText == null) Debug.LogError("❌ HealthText not assigned in Inspector!");
            else Debug.Log("✅ HealthText reference OK");

            // ПОИСК PlayerStats
            playerStats = FindFirstObjectByType<PlayerStats>();
            if (playerStats == null)
            {
                Debug.LogError("❌ PlayerStats not found in scene!");

                // Альтернативный поиск
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerStats = player.GetComponent<PlayerStats>();
                    if (playerStats != null) Debug.Log("✅ PlayerStats found via Player tag");
                }
            }
            else
            {
                Debug.Log($"✅ PlayerStats found: {playerStats.gameObject.name}");
                Debug.Log($"✅ Player Health: {playerStats.CurrentHealth}/{playerStats.MaxHealth}");
                Debug.Log($"✅ Player Exp: {playerStats.Experience}/{playerStats.ExperienceToNextLevel}");
            }

            // ПРОВЕРКА EVENT MANAGER
            if (EventManager.Instance == null)
                Debug.LogError("❌ EventManager Instance is null!");
            else
                Debug.Log("✅ EventManager Instance OK");

            // ПОДПИСКА НА СОБЫТИЯ С ОТЛАДКОЙ
            try
            {
                EventManager.Instance.StartListening(GameEventType.PlayerHealthChanged, OnHealthChanged);
                EventManager.Instance.StartListening(GameEventType.ExperienceGained, OnExperienceGained);
                EventManager.Instance.StartListening(GameEventType.PlayerLevelUp, OnLevelUp);
                Debug.Log("✅ Successfully subscribed to events");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Event subscription failed: {e.Message}");
            }

            // ПЕРВОНАЧАЛЬНОЕ ОБНОВЛЕНИЕ
            UpdateAllDisplays();
            Debug.Log("=== HUD INITIALIZATION COMPLETE ===");
        }

        private void UpdateAllDisplays()
        {
            UpdateHealthDisplay();
            UpdateExperienceDisplay();
            UpdateLevelDisplay();
        }

        private void OnHealthChanged(object healthData)
        {
            Debug.Log($"HUD: Health changed to {healthData}");
            UpdateHealthDisplay();
        }

        private void UpdateHealthDisplay()
        {
            if (playerStats != null && healthBar != null)
            {
                float healthPercent = (float)playerStats.CurrentHealth / playerStats.MaxHealth;
                healthBar.value = healthPercent;
                healthText.text = $"{playerStats.CurrentHealth}/{playerStats.MaxHealth}";

                Debug.Log($"Health Bar: {healthPercent:P0} ({playerStats.CurrentHealth}/{playerStats.MaxHealth})");
            }
        }

        private void OnExperienceGained(object expData)
        {
            Debug.Log($"HUD: Experience changed to {expData}");
            UpdateExperienceDisplay();
        }

        private void UpdateExperienceDisplay()
        {
            if (playerStats != null && expBar != null)
            {
                float expPercent = (float)playerStats.Experience / playerStats.ExperienceToNextLevel;
                expBar.value = expPercent;
                expText.text = $"{playerStats.Experience}/{playerStats.ExperienceToNextLevel} XP";

                Debug.Log($"Exp Bar: {expPercent:P0} ({playerStats.Experience}/{playerStats.ExperienceToNextLevel})");
            }
        }

        private void OnLevelUp(object levelData)
        {
            Debug.Log($"HUD: Level up to {levelData}");
            UpdateLevelDisplay();
            UpdateExperienceDisplay(); // Обновляем опыт после левел-апа
        }

        private void UpdateLevelDisplay()
        {
            if (playerStats != null && levelText != null)
            {
                levelText.text = $"Level {playerStats.Level}";
            }
        }

        // ТЕСТОВЫЕ КНОПКИ ДЛЯ ОТЛАДКИ
        private void Update()
        {
            // ВРЕМЕННО: принудительное обновление каждую секунду
            if (Time.frameCount % 60 == 0)
            {
                if (playerStats != null)
                {
                    float healthPercent = (float)playerStats.CurrentHealth / playerStats.MaxHealth;
                    healthBar.value = healthPercent;
                    Debug.Log($"Force update Health: {healthPercent:P0}");
                }
            }
        }
    }
}