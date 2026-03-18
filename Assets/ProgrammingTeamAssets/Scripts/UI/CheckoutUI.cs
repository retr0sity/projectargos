using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles the full checkout flow:
/// 1. Yes/No prompt
/// 2. Scan animation (items appear one by one)
/// 3. Afford check — success moves to inventory, failure shows cashier line + remove option
/// </summary>
public class CheckoutUI : MonoBehaviour
{
    public static CheckoutUI Instance { get; private set; }

    [Header("Dim Overlay")]
    public Image dimOverlay;                  // full-screen dark image, alpha ~0.6

    [Header("Yes/No Prompt")]
    public GameObject promptPanel;
    public TextMeshProUGUI promptText;        // "Pay $X.XX?"
    public Button yesButton;
    public Button noButton;

    [Header("Checkout Window")]
    public GameObject checkoutPanel;
    public Transform  itemListParent;         // vertical layout group
    public GameObject itemRowPrefab;          // prefab: TMP text showing "Item name — $price"
    public TextMeshProUGUI totalText;
    public TextMeshProUGUI balanceText;
    public TextMeshProUGUI cashierLine;       // "Sorry, you can't afford that."
    public Button removeLastButton;           // lets player remove last cart item
    public Button retryButton;               // retry after removing items
    public Button closeCheckoutButton;        // close without buying

    [Header("Scan Settings")]
    public float timeBetweenScans = 0.4f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        yesButton.onClick.AddListener(OnYes);
        noButton.onClick.AddListener(OnNo);
        removeLastButton.onClick.AddListener(OnRemoveLast);
        retryButton.onClick.AddListener(OnRetry);
        closeCheckoutButton.onClick.AddListener(OnCloseCheckout);

        dimOverlay.gameObject.SetActive(false);
        promptPanel.SetActive(false);
        checkoutPanel.SetActive(false);
    }

    // ── Step 1: Open prompt ───────────────────────────────────────

    public void OpenPrompt()
    {
        promptText.text = $"Pay ${CartManager.Instance.Total:F2}?";
        dimOverlay.gameObject.SetActive(true);
        promptPanel.SetActive(true);
    }

    void OnNo()
    {
        dimOverlay.gameObject.SetActive(false);
        promptPanel.SetActive(false);
    }

    void OnYes()
    {
        promptPanel.SetActive(false);
        StartCoroutine(ScanItems());
    }

    // ── Step 2: Scan animation ────────────────────────────────────

    IEnumerator ScanItems()
    {
        totalText.text   = $"Total: ${CartManager.Instance.Total:F2}";
balanceText.text = $"Your balance: ${PlayerWallet.Instance.balance:F2}";
        checkoutPanel.SetActive(true);
        cashierLine.gameObject.SetActive(false);
        removeLastButton.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);

        // Clear previous rows
        foreach (Transform child in itemListParent)
            Destroy(child.gameObject);

        // Scan items one by one
        foreach (var item in CartManager.Instance.items)
        {
            var row = Instantiate(itemRowPrefab, itemListParent);
            row.GetComponentInChildren<TextMeshProUGUI>().text =
                $"{item.productName}  —  ${item.price:F2}";
            yield return new WaitForSeconds(timeBetweenScans);
        }

        totalText.text   = $"Total: ${CartManager.Instance.Total:F2}";
        balanceText.text = $"Your balance: ${PlayerWallet.Instance.balance:F2}";

        // ── Step 3: Afford check ──────────────────────────────────
        if (PlayerWallet.Instance.CanAfford(CartManager.Instance.Total))
        {
            ConfirmPurchase();
        }
        else
        {
            ShowCantAfford();
        }
    }

    // ── Step 3a: Success ──────────────────────────────────────────

    void ConfirmPurchase()
    {
        // Check mood — did any item cost less than base price?
        bool gotADeal = false;
        foreach (var item in CartManager.Instance.items)
        {
            // We stored the daily price — compare against products in shelves
            // Simple rule: if total paid < sum of base prices, mood goes up
            gotADeal = true; // simplified: always up on successful purchase for now
        }

        PlayerWallet.Instance.Spend(CartManager.Instance.Total);
        InventoryManager.Instance.AddFromCart(); // clears cart too

        if (gotADeal) MoodManager.Instance.IncreaseMood();

        balanceText.text = $"Your balance: ${PlayerWallet.Instance.balance:F2}";
        closeCheckoutButton.gameObject.SetActive(true);
    }

    // ── Step 3b: Can't afford ─────────────────────────────────────

    void ShowCantAfford()
    {
        MoodManager.Instance.DecreaseMood();

        cashierLine.gameObject.SetActive(true);
        cashierLine.text = "Sorry, you don't have enough money.";
        removeLastButton.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);
    }

    void OnRemoveLast()
    {
        int last = CartManager.Instance.items.Count - 1;
        if (last < 0) return;
        CartManager.Instance.RemoveItem(last);

        // Remove last row from UI
        if (itemListParent.childCount > 0)
            Destroy(itemListParent.GetChild(itemListParent.childCount - 1).gameObject);

        totalText.text   = $"Total: ${CartManager.Instance.Total:F2}";
        balanceText.text = $"Your balance: ${PlayerWallet.Instance.balance:F2}";
    }

    void OnRetry()
    {
        if (CartManager.Instance.items.Count == 0)
        {
            OnCloseCheckout();
            return;
        }

        cashierLine.gameObject.SetActive(false);
        removeLastButton.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);

        if (PlayerWallet.Instance.CanAfford(CartManager.Instance.Total))
            ConfirmPurchase();
        else
            ShowCantAfford();
    }

    void OnCloseCheckout()
    {
        checkoutPanel.SetActive(false);
        dimOverlay.gameObject.SetActive(false);
        closeCheckoutButton.gameObject.SetActive(false);
    }
}
