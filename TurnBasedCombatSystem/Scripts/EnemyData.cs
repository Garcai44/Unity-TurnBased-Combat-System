using UnityEngine;

/// <summary>
/// Blueprint for an enemy. Defines base stats and loot tables.
/// Use this ScriptableObject to create different enemy types without duplicating logic.
/// </summary>
[CreateAssetMenu(fileName = "NewEnemyData", menuName = "TurnBasedCombat/Enemy Data", order = 1)]
public class EnemyData : ScriptableObject
{
    [Header("General Info")]
    [Tooltip("The display name of the enemy.")]
    public string enemyName = "Enemy Name";

    [Header("Combat Stats")]
    [Tooltip("Maximum health points the enemy starts with.")]
    public int maxHp = 50;
    
    [Tooltip("Base damage dealt by this enemy per attack.")]
    public int baseDamage = 10; 

    [Header("Loot / Rewards")]
    [Tooltip("Minimum currency dropped when defeated.")]
    public int minCurrencyDrop = 10;        
    
    [Tooltip("Maximum currency dropped when defeated.")]
    public int maxCurrencyDrop = 20;        
    
    [Tooltip("Probability (0.0 to 1.0) of dropping a rare item.")]
    [Range(0f, 1f)]
    public float rareItemDropChance = 0.05f; 
}