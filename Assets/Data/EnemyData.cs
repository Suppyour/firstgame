using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int maxHealth = 10;
    public int damage = 5;
    public float moveSpeed = 2f;
    public int experienceValue = 10;

    // Добавьте эти свойства для удобства
    public int MaxHealth => maxHealth;
    public int Damage => damage;
    public float MoveSpeed => moveSpeed;
    public int ExperienceValue => experienceValue;
}