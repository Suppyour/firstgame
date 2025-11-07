using UnityEngine;
using Core;

namespace Characters
{
    public class PlayerStats : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int currentHealth;
        [SerializeField] private int level = 1;
        [SerializeField] private int experience = 0;
        [SerializeField] private int experienceToNextLevel = 100;

        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;
        public int Level => level;
        public int Experience => experience;
        public int ExperienceToNextLevel => experienceToNextLevel;
        public float MoveSpeedMultiplier { get; private set; } = 1f;
        public float DamageMultiplier { get; private set; } = 1f;

        private void Start()
        {
            currentHealth = maxHealth;

            // ТРИГГЕРИМ НАЧАЛЬНОЕ СОСТОЯНИЕ
            EventManager.Instance?.TriggerEvent(GameEventType.PlayerHealthChanged, currentHealth);
            EventManager.Instance?.TriggerEvent(GameEventType.ExperienceChanged, experience); // ← ИСПРАВЛЕНО!

            // ПОДПИСЫВАЕМСЯ НА СОБЫТИЯ
            EventManager.Instance?.StartListening(GameEventType.ExperienceGained, OnExperienceGained);
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            Debug.Log($"Player took {damage} damage. Health: {currentHealth}/{maxHealth}");

            EventManager.Instance?.TriggerEvent(GameEventType.PlayerHealthChanged, currentHealth);

            if (currentHealth <= 0) Die();
        }

        private void OnExperienceGained(object expAmount)
        {
            // ЗАЩИТА ОТ НЕПРАВИЛЬНЫХ ДАННЫХ
            if (expAmount is int exp && exp > 0)
            {
                experience += exp;
                Debug.Log($"✅ Gained {exp} XP. Total: {experience}/{experienceToNextLevel}");

                // ВАЖНО: Триггерим ДРУГОЕ событие для HUD!
                EventManager.Instance?.TriggerEvent(GameEventType.ExperienceChanged, experience); // ← ИСПРАВЛЕНО!

                if (experience >= experienceToNextLevel)
                {
                    LevelUp();
                }
            }
            else
            {
                Debug.LogError($"❌ Invalid experience data: {expAmount} (type: {expAmount?.GetType()})");
            }
        }

        private void LevelUp()
        {
            level++;
            experience -= experienceToNextLevel;
            experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.5f);

            Debug.Log($"Level Up! Now level {level}. Next level at {experienceToNextLevel} XP");

            EventManager.Instance?.TriggerEvent(GameEventType.PlayerLevelUp, level);
            EventManager.Instance?.TriggerEvent(GameEventType.ExperienceChanged, experience); // ← ИСПРАВЛЕНО!
        }

        private void Die()
        {
            Debug.Log("Player died!");
            GameManager.Instance?.PlayerDied();
        }

        public void ApplyUpgrade(Upgrade upgrade)
        {
            Debug.Log($"Applying upgrade: {upgrade.name}");

            switch (upgrade.Type)
            {
                case UpgradeType.Health:
                    maxHealth += upgrade.Value;
                    currentHealth += upgrade.Value;
                    EventManager.Instance?.TriggerEvent(GameEventType.PlayerHealthChanged, currentHealth);
                    break;
                case UpgradeType.Damage:
                    DamageMultiplier += upgrade.Value / 100f;
                    break;
                case UpgradeType.MoveSpeed:
                    MoveSpeedMultiplier += upgrade.Value / 100f;
                    break;
                case UpgradeType.NewWeapon:
                    Debug.Log($"New weapon unlocked: {upgrade.name}");
                    break;
            }
        }
    }
}