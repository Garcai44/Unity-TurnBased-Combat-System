using UnityEngine;

/// <summary>
/// Represents the live instance of an enemy during combat.
/// Tracks volatile data like current HP based on the immutable EnemyData.
/// </summary>
public class EnemyStats
{
    /// <summary> Reference to the base stats and definitions. </summary>
    public EnemyData data; 
    
    /// <summary> The enemy's current health points. </summary>
    public int currentHp;

    /// <summary>
    /// Initializes a new live enemy based on the provided blueprint.
    /// </summary>
    /// <param name="enemyData">The ScriptableObject containing base stats.</param>
    public EnemyStats(EnemyData enemyData)
    {
        data = enemyData;
        currentHp = enemyData != null ? enemyData.maxHp : 0;
    }

    /// <summary> Checks if the enemy's health has dropped to zero or below. </summary>
    public bool IsDead() => currentHp <= 0;

    /// <summary> Reduces the enemy's current HP by the specified amount. </summary>
    public void TakeDamage(int amount)
    {
        currentHp = Mathf.Max(0, currentHp - amount);
    }

    /// <summary> Retrieves the damage this enemy will deal this turn. </summary>
    public int CalculateDamage()
    {
        if (data == null) return 0;
        // Future expansion: Add critical hits or buffs here.
        return data.baseDamage;
    }

    /// <summary> Restores health up to the maximum HP limit. </summary>
    public void Heal(int amount)
    {
        if (data == null) return;
        currentHp = Mathf.Min(data.maxHp, currentHp + amount);
    }
}