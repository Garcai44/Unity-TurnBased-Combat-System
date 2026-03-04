using UnityEngine;
using TMPro; // Asegúrate de tener importado TextMeshPro

public class CombatTester : MonoBehaviour
{
    [Header("Referencias Base")]
    public TurnBasedCombatManager combatManager;
    public PlayerStats player;
    public EnemyData testEnemy;

    [Header("Textos de la UI")]
    public TMP_Text logText;
    public TMP_Text playerHpText;
    public TMP_Text enemyHpText;

    void Start()
    {
        // 1. Arrancar el combate nada más darle al Play
        if (testEnemy != null && player != null)
        {
            combatManager.StartCombat(player, testEnemy);
        }
    }

    // 2. Estos métodos los llamará el Manager a través de los UnityEvents
    public void UpdateLog(string msg) { if (logText) logText.text = msg; }
    
    public void UpdatePlayerUI(int currentHp, int maxHp) 
    { 
        if (playerHpText) playerHpText.text = $"Player HP: {currentHp}/{maxHp}"; 
    }
    
    public void UpdateEnemyUI(int currentHp, int maxHp) 
    { 
        if (enemyHpText) enemyHpText.text = $"Enemy HP: {currentHp}/{maxHp}"; 
    }
}