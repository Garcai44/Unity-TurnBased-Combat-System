using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary> Available actions a player can take during their turn. </summary>
public enum CombatAction { None, Attack, Heal, Flee }

/// <summary>
/// Core engine for the Turn-Based Combat System.
/// Handles the turn cycle, delays, and fires UnityEvents to update external UI.
/// </summary>
public class TurnBasedCombatManager : MonoBehaviour
{
    [Header("Combat Settings")]
    [Tooltip("Delay in seconds after an action is performed before the next turn starts.")]
    public float postActionDelay = 0.5f;
    
    [Tooltip("How long a combat log message stays on screen before proceeding.")]
    public float messageDisplayTime = 2.0f;
    
    [Tooltip("Flat amount of HP restored when using the Heal action.")]
    public int baseHealAmount = 15;
    
    [Tooltip("Probability (0.0 to 1.0) of successfully fleeing combat.")]
    [Range(0f, 1f)] 
    public float fleeSuccessChance = 0.6f;

    [Header("UI Events (Connect in Inspector)")]
    public UnityEvent<string> OnMessageUpdated;
    public UnityEvent<int, int> OnPlayerHealthUpdated; // Passes (Current HP, Max HP)
    public UnityEvent<int, int> OnEnemyHealthUpdated;  // Passes (Current HP, Max HP)
    public UnityEvent<bool> OnCombatEnded;             // Passes true if player won

    // Internal Combat State
    private PlayerStats player;
    private EnemyStats currentEnemy;
    private CombatAction currentAction = CombatAction.None;
    private bool isCombatActive = false;

    /// <summary>
    /// Initializes and starts a new combat encounter.
    /// </summary>
    /// <param name="playerData">Reference to the player's stats.</param>
    /// <param name="enemyData">The blueprint of the enemy being fought.</param>
    public void StartCombat(PlayerStats playerData, EnemyData enemyData)
    {
        if (isCombatActive) return;

        player = playerData;
        currentEnemy = new EnemyStats(enemyData); 
        isCombatActive = true;

        UpdateHealthEvents();
        ShowMessage($"Encounter: {currentEnemy.data.enemyName}");

        StartCoroutine(CombatRoutine());
    }

    /// <summary> Main loop controlling the turn order. </summary>
    private IEnumerator CombatRoutine()
    {
        while (player.currentHp > 0 && !currentEnemy.IsDead())
        {
            // --- 1. Player Turn ---
            currentAction = CombatAction.None;
            yield return ShowMessageAndWait("Your turn — choose an action");
            
            // Pauses the coroutine until the UI buttons change 'currentAction'
            yield return new WaitUntil(() => currentAction != CombatAction.None);

            switch (currentAction)
            {
                case CombatAction.Attack:
                    int dmg = player.CalculateDamage();
                    currentEnemy.TakeDamage(dmg);
                    yield return ShowMessageAndWait($"You attack for {dmg} damage!");
                    break;

                case CombatAction.Heal:
                    player.Heal(baseHealAmount);
                    yield return ShowMessageAndWait($"You heal +{baseHealAmount} HP");
                    break;

                case CombatAction.Flee:
                    if (Random.value < fleeSuccessChance)
                    {
                        // Successfully fled. Pass false (didn't win) but true to 'fled'
                        EndCombat(false, true);
                        yield break; // Exits the coroutine entirely
                    }
                    else
                    {
                        // Failed to flee. You lose your turn, but don't take extra direct damage here.
                        yield return ShowMessageAndWait("Flee failed! You couldn't escape!");
                    }
                    break;
            }

            UpdateHealthEvents();
            if (currentEnemy.IsDead() || !isCombatActive) break;
            yield return new WaitForSeconds(postActionDelay);

            // --- 2. Enemy Turn ---
            int enemyAttack = currentEnemy.CalculateDamage();
            player.TakeDamage(enemyAttack);
            yield return ShowMessageAndWait($"Enemy attacks for {enemyAttack} damage!");

            UpdateHealthEvents();
            yield return new WaitForSeconds(postActionDelay);
        }

        // Combat resolution (only if we didn't flee and break out early)
        if (isCombatActive)
        {
            bool playerWon = player.currentHp > 0;
            EndCombat(playerWon);
        }
    }

    /// <summary> Handles logic for finishing the battle and distributing loot. </summary>
    private void EndCombat(bool playerWon, bool fled = false)
    {
        isCombatActive = false;
        currentAction = CombatAction.None;

        if (playerWon)
        {
            // Calculate and apply loot
            int lootCurrency = Random.Range(currentEnemy.data.minCurrencyDrop, currentEnemy.data.maxCurrencyDrop + 1);
            player.currency += lootCurrency;
            
            if (Random.value < currentEnemy.data.rareItemDropChance)
            {
                player.rareItems++;
                ShowMessage($"Victory! +{lootCurrency} coins and 1 Rare Item!");
            }
            else
            {
                ShowMessage($"Victory! +{lootCurrency} coins.");
            }
        }
        else if (fled)
        {
            ShowMessage("You fled safely!");
        }
        else
        {
            ShowMessage("Defeat...");
        }

        OnCombatEnded?.Invoke(playerWon);
    }

    // --- UI Helpers ---

    private void UpdateHealthEvents()
    {
        OnPlayerHealthUpdated?.Invoke(player.currentHp, player.maxHp);
        
        if (currentEnemy != null)
        {
            OnEnemyHealthUpdated?.Invoke(currentEnemy.currentHp, currentEnemy.data.maxHp);
        }
    }

    private void ShowMessage(string msg) => OnMessageUpdated?.Invoke(msg);

    private IEnumerator ShowMessageAndWait(string msg)
    {
        ShowMessage(msg);
        yield return new WaitForSeconds(messageDisplayTime);
    }

    // --- Public methods for UI Buttons ---
    
    /// <summary> Triggers the Attack action. Link this to your UI Button. </summary>
    public void SelectActionAttack() { if (isCombatActive) currentAction = CombatAction.Attack; }
    
    /// <summary> Triggers the Heal action. Link this to your UI Button. </summary>
    public void SelectActionHeal() { if (isCombatActive) currentAction = CombatAction.Heal; }
    
    /// <summary> Triggers the Flee action. Link this to your UI Button. </summary>
    public void SelectActionFlee() { if (isCombatActive) currentAction = CombatAction.Flee; }
}