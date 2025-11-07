using UnityEngine;

namespace Core
{
    public enum UpgradeType
    {
        Health,
        Damage,
        MoveSpeed,
        NewWeapon
    }

    [System.Serializable]
    public class Upgrade
    {
        public string name;
        public string description;
        public UpgradeType Type;
        public int Value;

        public Upgrade(string name, string description, UpgradeType type, int value)
        {
            this.name = name;
            this.description = description;
            this.Type = type;
            this.Value = value;
        }
    }

    public class UpgradeSystem : MonoBehaviour
    {
        [SerializeField]
        private Upgrade[] availableUpgrades =
        {
            new Upgrade("Max Health", "Increase maximum health by 20", UpgradeType.Health, 20),
            new Upgrade("Damage Boost", "Increase damage by 15%", UpgradeType.Damage, 15),
            new Upgrade("Movement Speed", "Increase movement speed by 10%", UpgradeType.MoveSpeed, 10),
            new Upgrade("Health Regeneration", "Increase maximum health by 15", UpgradeType.Health, 15),
            new Upgrade("Critical Strike", "Increase damage by 20%", UpgradeType.Damage, 20)
        };

        public Upgrade[] GetRandomUpgrades(int count)
        {
            if (count >= availableUpgrades.Length)
                return availableUpgrades;

            Upgrade[] randomUpgrades = new Upgrade[count];
            System.Random rng = new System.Random();

            // Простая реализация выбора случайных улучшений
            for (int i = 0; i < count; i++)
            {
                randomUpgrades[i] = availableUpgrades[rng.Next(availableUpgrades.Length)];
            }

            return randomUpgrades;
        }
    }
}