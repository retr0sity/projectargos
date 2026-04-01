using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Dialogue-driven cashier checkout flow.
/// After the "can't afford" line, shows the Market_Cart_Contents panel so the
/// player can choose which items to put back. Closing the panel re-runs the
/// checkout prompt so the player can retry.
/// </summary>
public class CashierInteractable : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private string speakerName = "Cashier";
    [SerializeField] private string checkoutPromptFormat = "Are you done shopping and want to pay ${0:F2}?";
    [SerializeField] private string thankYouLine = "Thank you.";
    [SerializeField] private string insufficientFundsLine = "Sorry, not enough money.";

    [Header("Cart Contents Panel")]
    [Tooltip("The Market_Cart_Contents panel in the Market scene.")]
    [SerializeField] private GameObject cartContentsPanel;
    [Tooltip("The 'Content' transform inside the panel's Scroll View.")]
    [SerializeField] private Transform cartContentsParent;
    [Tooltip("The row prefab (the 'Panel' child inside Content).")]
    [SerializeField] private GameObject cartRowPrefab;
    [Tooltip("Optional — a Done/Continue button on the panel. If unassigned, each removal auto-closes and re-runs the checkout prompt.")]
    [SerializeField] private Button cartDoneButton;

    private bool isInteractionInProgress;

    void Start()
    {
        if (gameObject.tag != "Interactable")
            gameObject.tag = "Interactable";

        ResolveCartPanel();

        if (cartContentsPanel != null)
            cartContentsPanel.SetActive(false);
    }

    /// <summary>
    /// Finds Market_Cart_Contents and its Content child in the active scene at runtime.
    /// Uses scene.GetRootGameObjects() so it works even when the panel starts inactive.
    /// cartContentsParent is always re-derived from the panel to avoid stale Inspector values.
    /// </summary>
    void ResolveCartPanel()
    {
        Debug.Log($"[Cashier] ResolveCartPanel on '{gameObject.name}', prefab={cartRowPrefab}");

        if (cartContentsPanel == null)
        {
            foreach (var root in gameObject.scene.GetRootGameObjects())
            {
                Transform found = FindDeep(root.transform, "Market_Cart_Contents");
                if (found != null) { cartContentsPanel = found.gameObject; break; }
            }
        }

        // Always re-derive from the panel so stale Inspector assignments are ignored
        if (cartContentsPanel != null)
        {
            Transform content = cartContentsPanel.transform.Find("Scroll View/Viewport/Content");
            if (content != null) cartContentsParent = content;
        }

        // Find prefab by name from loaded assets if still unassigned
        if (cartRowPrefab == null)
        {
            foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (go.name == "PrefabRemovePanel" && !go.scene.IsValid())
                {
                    cartRowPrefab = go;
                    break;
                }
            }
        }

        if (cartContentsPanel == null)
            Debug.LogWarning("[Cashier] Market_Cart_Contents not found in scene.");
        if (cartContentsParent == null)
            Debug.LogWarning("[Cashier] Content not found inside Market_Cart_Contents.");
        if (cartRowPrefab == null)
            Debug.LogWarning("[Cashier] PrefabRemovePanel not found — make sure the prefab exists in the project.");
    }

    static Transform FindDeep(Transform parent, string name)
    {
        if (parent.name == name) return parent;
        foreach (Transform child in parent)
        {
            Transform result = FindDeep(child, name);
            if (result != null) return result;
        }
        return null;
    }

    public void OnInteract()
    {
        if (isInteractionInProgress)
            return;

        if (CartManager.Instance == null || CartManager.Instance.items.Count == 0)
        {
            Debug.Log("[Cashier] Cart is empty.");
            return;
        }

        if (DialogueManager.Instance == null)
        {
            Debug.LogError("DialogueManager not found in scene!");
            return;
        }

        if (PlayerWallet.Instance == null)
        {
            Debug.LogError("PlayerWallet not found in scene!");
            return;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager not found in scene!");
            return;
        }

        isInteractionInProgress = true;
        ShowCheckoutPrompt();
    }

    void ShowCheckoutPrompt()
    {
        if (CartManager.Instance == null || CartManager.Instance.items.Count == 0)
        {
            EndInteraction();
            return;
        }

        string prompt = string.Format(checkoutPromptFormat, CartManager.Instance.Total);
        DialogueManager.Instance.ShowChoices(
            new[] { "Yes", "No" },
            OnCheckoutChoice,
            prompt);
    }

    void OnCheckoutChoice(int choiceIndex)
    {
        if (choiceIndex != 0)
        {
            EndInteraction();
            return;
        }

        if (CartManager.Instance == null || PlayerWallet.Instance == null)
        {
            Debug.LogError("Missing cart or wallet manager.");
            EndInteraction();
            return;
        }

        if (PlayerWallet.Instance.CanAfford(CartManager.Instance.Total))
        {
            CompleteCheckout();
            return;
        }

        if (MoodManager.Instance != null)
            MoodManager.Instance.DecreaseMood();

        DialogueManager.Instance.StartDialogue(
            new[] { insufficientFundsLine },
            speakerName,
            ShowCartContentsPanel);
    }

    void CompleteCheckout()
    {
        if (CartManager.Instance == null || PlayerWallet.Instance == null || InventoryManager.Instance == null)
        {
            Debug.LogError("Missing checkout managers.");
            EndInteraction();
            return;
        }

        float total = CartManager.Instance.Total;
        PlayerWallet.Instance.Spend(total);
        InventoryManager.Instance.AddFromCart();

        if (MoodManager.Instance != null)
            MoodManager.Instance.IncreaseMood();

        DialogueManager.Instance.StartDialogue(
            new[] { thankYouLine },
            speakerName,
            EndInteraction);
    }

    // ── Cart Contents Panel ───────────────────────────────────────

    void ShowCartContentsPanel()
    {
        if (cartContentsPanel == null || cartContentsParent == null || cartRowPrefab == null)
        {
            Debug.LogWarning($"[Cashier] Cart contents panel not configured on '{gameObject.name}': panel={cartContentsPanel}, parent={cartContentsParent}, prefab={cartRowPrefab}");
            EndInteraction();
            return;
        }

        PopulateCartContents();
        cartContentsPanel.SetActive(true);
        DialogueManager.Instance.BeginExternalUI();

        if (cartDoneButton != null)
        {
            cartDoneButton.onClick.RemoveAllListeners();
            cartDoneButton.onClick.AddListener(OnCartDone);
        }
    }

    void PopulateCartContents()
    {
        foreach (Transform child in cartContentsParent)
            Destroy(child.gameObject);

        foreach (CartManager.CartItem item in CartManager.Instance.items)
        {
            var row = Instantiate(cartRowPrefab, cartContentsParent);

            var nameText = row.transform.Find("NameOfProductText")?.GetComponent<TextMeshProUGUI>();
            if (nameText != null)
                nameText.text = item.productName;

            var removeBtn = row.transform.Find("RemoveButton")?.GetComponent<Button>();
            if (removeBtn != null)
            {
                removeBtn.onClick.RemoveAllListeners();
                removeBtn.onClick.AddListener(() => OnCartRemoveItem(item));
            }
        }
    }

    void OnCartRemoveItem(CartManager.CartItem item)
    {
        CartManager.Instance.items.Remove(item);

        if (CartManager.Instance.items.Count == 0)
        {
            CloseCartContentsPanel();
            EndInteraction();
            return;
        }

        PopulateCartContents();

        if (cartDoneButton == null)
        {
            CloseCartContentsPanel();
            ShowCheckoutPrompt();
        }
    }

    void OnCartDone()
    {
        CloseCartContentsPanel();
        ShowCheckoutPrompt();
    }

    void CloseCartContentsPanel()
    {
        if (cartContentsPanel != null && cartContentsPanel.activeSelf)
        {
            cartContentsPanel.SetActive(false);
            DialogueManager.Instance.EndExternalUI();
        }
    }

    void EndInteraction()
    {
        CloseCartContentsPanel();
        isInteractionInProgress = false;
    }
}
