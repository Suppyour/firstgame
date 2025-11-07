using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Game/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public int damage = 5;
    public float attackCooldown = 1f;
    public float attackRange = 3f;

    // Добавьте свойства для удобства (или используйте поля напрямую)
    public int Damage => damage;
    public float AttackCooldown => attackCooldown;
    public float AttackRange => attackRange;
}