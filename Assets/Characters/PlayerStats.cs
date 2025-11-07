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
            EventManager.Instance?.TriggerEvent(GameEventType.ExperienceGained, 0);

            // ПОДПИСЫВАЕМСЯ НА СОБЫТИЯ
            EventManager.Instance?.StartListening(GameEventType.ExperienceGained, OnExperienceGained);
        }

        public void TakeDamage(int damage)
        {
            if (damage <= 0) return;

            currentHealth -= damage;
            Debug.Log($"Player took {damage} damage. Health: {currentHealth}/{maxHealth}");

            // ВЫЗЫВАЕМ СОБЫТИЕ ПРИ ЛЮБОМ ИЗМЕНЕНИИ ЗДОРОВЬЯ
            EventManager.Instance?.TriggerEvent(GameEventType.PlayerHealthChanged, currentHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void OnExperienceGained(object expAmount)
        {
            int exp = (int)expAmount;
            if (exp <= 0) return;

            experience += exp;
            Debug.Log($"Gained {exp} XP. Total: {experience}/{experienceToNextLevel}");

            // ВЫЗЫВАЕМ СОБЫТИЕ ОПЫТА
            EventManager.Instance?.TriggerEvent(GameEventType.ExperienceGained, experience);

            if (experience >= experienceToNextLevel)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            level++;
            experience -= experienceToNextLevel;
            experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.5f);

            Debug.Log($"Level Up! Now level {level}. Next level at {experienceToNextLevel} XP");

            // ВЫЗЫВАЕМ СОБЫТИЕ ЛЕВЕЛ-АПА
            EventManager.Instance?.TriggerEvent(GameEventType.PlayerLevelUp, level);

            // ТРИГГЕРИМ ОБНОВЛЕНИЕ ОПЫТА ДЛЯ HUD
            EventManager.Instance?.TriggerEvent(GameEventType.ExperienceGained, experience);
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