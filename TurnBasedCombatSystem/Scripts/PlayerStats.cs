using UnityEngine;

/// <summary>
/// Manages the player's combat statistics, health, and inventory resources.
/// Attach this component to the Player GameObject.
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [Header("Health Setup")]
    public int maxHp = 100;
    public int currentHp = 100;

    [Header("Combat Base Stats")]
    [Tooltip("Base damage dealt by the player before modifiers.")]
    public int baseDamage = 20; 

    [Header("Inventory / Resources")]
    [Tooltip("Standard currency used for shops/upgrades.")]
    public int currency = 0;    
    
    [Tooltip("Rare collectible items dropped by bosses or specific enemies.")]
    public int rareItems = 0;   

    private void Start()
    {
        // Ensure starting HP does not exceed the maximum allowed
        currentHp = Mathf.Min(currentHp, maxHp);
    }

    /// <summary>
    /// Calculates the total damage output for the current turn.
    /// </summary>
    /// <returns>Total damage calculated.</returns>
    public int CalculateDamage()
    {
        int totalDamage = baseDamage;
        // Note: Implement equipment, buffs, or crew modifiers here in the future
        return totalDamage;
    }

    /// <summary> Applies incoming damage to the player's health pool. </summary>
    public void TakeDamage(int amount)
    {
        currentHp -= amount;
        if (currentHp < 0) currentHp = 0;
    }

    /// <summary> Restores the player's health without exceeding the maximum HP. </summary>
    public void Heal(int baseAmount)
    {
        int totalHeal = baseAmount;
        // Note: Implement healing modifiers (e.g., better potions) here
        currentHp += totalHeal;
        
        if (currentHp > maxHp) currentHp = maxHp;
    }
}