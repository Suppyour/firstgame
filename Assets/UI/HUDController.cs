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
            Debug.Log("HUDController started");

            playerStats = FindFirstObjectByType<PlayerStats>();

            if (playerStats == null)
            {
                Debug.LogError("PlayerStats not found!");
                return;
            }

            // ПОДПИСКА НА СОБЫТИЯ
            EventManager.Instance.StartListening(GameEventType.PlayerHealthChanged, OnHealthChanged);
            EventManager.Instance.StartListening(GameEventType.ExperienceGained, OnExperienceGained);
            EventManager.Instance.StartListening(GameEventType.PlayerLevelUp, OnLevelUp);

            // ОБНОВЛЯЕМ НАЧАЛЬНЫЕ ЗНАЧЕНИЯ
            UpdateAllDisplays();
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
            // Тестовые клавиши для проверки
            if (Input.GetKeyDown(KeyCode.F1))
            {
                Debug.Log("TEST: Giving 25 experience");
                EventManager.Instance.TriggerEvent(GameEventType.ExperienceGained, 25);
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                Debug.Log("TEST: Taking 10 damage");
                playerStats?.TakeDamage(10);
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                Debug.Log("TEST: Manual HUD update");
                UpdateAllDisplays();
            }
        }
    }
}