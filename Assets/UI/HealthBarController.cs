using UnityEngine;
using UnityEngine.UI;
using Core;

namespace UI
{
    public class HealthBarController : MonoBehaviour
    {
        [SerializeField] private Slider healthBar;
        [SerializeField] private Image fillImage;
        [SerializeField] private Gradient healthGradient;

        private void Start()
        {
            healthBar = GetComponent<Slider>();
            EventManager.Instance.StartListening(GameEventType.PlayerHealthChanged, OnHealthChanged);
        }

        private void OnHealthChanged(object healthData)
        {
            int currentHealth = (int)healthData;
            Characters.PlayerStats playerStats = FindFirstObjectByType<Characters.PlayerStats>();

            if (playerStats != null)
            {
                float healthPercent = (float)currentHealth / playerStats.MaxHealth;
                healthBar.value = healthPercent;

                // Меняем цвет от зеленого к красному
                fillImage.color = healthGradient.Evaluate(healthPercent);
            }
        }
    }
}