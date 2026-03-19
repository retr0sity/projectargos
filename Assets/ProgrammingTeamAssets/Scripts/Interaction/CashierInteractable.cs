using UnityEngine;

/// <summary>
/// Dialogue-driven cashier checkout flow.
/// </summary>
public class CashierInteractable : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private string speakerName = "Cashier";
    [SerializeField] private string checkoutPromptFormat = "Are you done shopping and want to pay ${0:F2}?";
    [SerializeField] private string thankYouLine = "Thank you.";
    [SerializeField] private string insufficientFundsLine = "Sorry, not enough money.";
    [SerializeField] private string removeLastItemPromptFormat = "Do you want to put back {0} for ${1:F2}?";

    private bool isInteractionInProgress;

    void Start()
    {
        if (gameObject.tag != "Interactable")
            gameObject.tag = "Interactable";
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
            ShowRemoveLastItemPrompt);
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

    void ShowRemoveLastItemPrompt()
    {
        if (CartManager.Instance == null || CartManager.Instance.items.Count == 0)
        {
            EndInteraction();
            return;
        }

        CartManager.CartItem lastItem = CartManager.Instance.items[CartManager.Instance.items.Count - 1];
        string prompt = string.Format(
            removeLastItemPromptFormat,
            lastItem.productName,
            lastItem.price);

        DialogueManager.Instance.ShowChoices(
            new[] { "Yes", "No" },
            OnRemoveLastItemChoice,
            prompt);
    }

    void OnRemoveLastItemChoice(int choiceIndex)
    {
        if (choiceIndex != 0)
        {
            EndInteraction();
            return;
        }

        if (CartManager.Instance == null || CartManager.Instance.items.Count == 0)
        {
            EndInteraction();
            return;
        }

        CartManager.Instance.RemoveItem(CartManager.Instance.items.Count - 1);

        if (CartManager.Instance.items.Count == 0)
        {
            EndInteraction();
            return;
        }

        ShowCheckoutPrompt();
    }

    void EndInteraction()
    {
        isInteractionInProgress = false;
    }
}
