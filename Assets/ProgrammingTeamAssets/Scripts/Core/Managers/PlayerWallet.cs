using UnityEngine;

/// <summary>
/// Tracks player money across the whole game. DontDestroyOnLoad singleton.
/// </summary>
public class PlayerWallet : MonoBehaviour
{
    public static PlayerWallet Instance { get; private set; }

    [Header("Starting Balance")]
    public float balance = 20f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool CanAfford(float amount) => balance >= amount;

    public void Spend(float amount)
    {
        balance -= amount;
        balance  = Mathf.Max(balance, 0f);
        Debug.Log($"[Wallet] Spent ${amount:F2} — Balance: ${balance:F2}");
    }

    public void AddFunds(float amount)
    {
        balance += amount;
        Debug.Log($"[Wallet] Added ${amount:F2} — Balance: ${balance:F2}");
    }
}
