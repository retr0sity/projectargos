using System.Collections.Generic;
using Core.Managers;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryWalletPanel : MonoBehaviour
{
    // ── Private state ──────────────────────────────────────────────────
    private GameObject  _panelRoot;
    private bool        _isOpen = false;

    private TextMeshProUGUI _walletText;
    private Transform       _itemListParent;

    // ── Lifecycle ──────────────────────────────────────────────────────
    void Start()
    {
        var input = InputManager.Instance;
        if (input != null)
            input.InventoryEvent += TogglePanel;
        else
            Debug.LogWarning("[InventoryWalletPanel] No InputManager found.");

        BuildPanelUI();
        _panelRoot.SetActive(false);
    }

    void OnDestroy()
    {
        var input = InputManager.Instance;
        if (input != null)
            input.InventoryEvent -= TogglePanel;
    }

    // ── Toggle ─────────────────────────────────────────────────────────
    void TogglePanel()
{
    _isOpen = !_isOpen;
    _panelRoot.SetActive(_isOpen);
    Debug.Log($"[InventoryPanel] Toggled — isOpen: {_isOpen}, items: {InventoryManager.Instance?.items.Count}");

    if (_isOpen)
        RefreshContent();
}

    // ── Content refresh ────────────────────────────────────────────────
    void RefreshContent()
{
    Debug.Log($"[InventoryPanel] RefreshContent called — itemListParent null: {_itemListParent == null}, InventoryManager null: {InventoryManager.Instance == null}");
    if (_walletText != null && PlayerWallet.Instance != null)
        _walletText.text = $"Balance:  ${PlayerWallet.Instance.balance:F2}";

    if (_itemListParent == null || InventoryManager.Instance == null) return;

    // Immediate destroy — no deferred cleanup
    for (int i = _itemListParent.childCount - 1; i >= 0; i--)
        DestroyImmediate(_itemListParent.GetChild(i).gameObject);

    List<InventoryManager.InventoryItem> items = InventoryManager.Instance.items;

    if (items.Count == 0)
    {
        AddRow(_itemListParent, "  (empty)", Color.gray);
    }
    else
    {
        foreach (var item in items)
        {
            string kindTag = item.itemKind == InventoryItemKind.CookedMeal ? "[Meal]" : "[Ing]";
            string qualTag = item.itemKind == InventoryItemKind.CookedMeal ? $" — {item.qualityTier}" : "";
            string label   = $"{kindTag}  {item.productName}{qualTag}";
            Color rowColor = item.itemKind == InventoryItemKind.CookedMeal
                ? new Color(1f, 0.85f, 0.4f) : Color.white;
            AddRow(_itemListParent, label, rowColor);
        }
    }

    // Force full layout resolution top-down
    Canvas.ForceUpdateCanvases();
    LayoutRebuilder.ForceRebuildLayoutImmediate(_panelRoot.GetComponent<RectTransform>());
}

        void AddRow(Transform parent, string text, Color color)
{
    GameObject row = new GameObject("Row", typeof(RectTransform));
    row.transform.SetParent(parent, false);

    LayoutElement le = row.AddComponent<LayoutElement>();
    le.minHeight       = 22f;
    le.preferredHeight = 22f;

    TextMeshProUGUI tmp = row.AddComponent<TextMeshProUGUI>();
    tmp.text            = text;
    tmp.fontSize        = 13f;
    tmp.color           = color;
    tmp.enableWordWrapping = false;
}

    // ── Placeholder UI builder ─────────────────────────────────────────
    void BuildPanelUI()
    {
        Canvas canvas = gameObject.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 20;
            gameObject.AddComponent<CanvasScaler>().uiScaleMode =
                CanvasScaler.ScaleMode.ScaleWithScreenSize;
            gameObject.AddComponent<GraphicRaycaster>();
        }

        // Root panel — centred
        _panelRoot = new GameObject("InventoryWalletPanel", typeof(RectTransform));
        _panelRoot.transform.SetParent(transform, false);
        RectTransform pr = _panelRoot.GetComponent<RectTransform>();
        pr.anchorMin        = new Vector2(0.5f, 0.5f);
        pr.anchorMax        = new Vector2(0.5f, 0.5f);
        pr.pivot            = new Vector2(0.5f, 0.5f);
        pr.sizeDelta        = new Vector2(340f, 460f);
        pr.anchoredPosition = Vector2.zero;

        _panelRoot.AddComponent<Image>().color = new Color(0.05f, 0.05f, 0.1f, 0.92f);

        // Title
        MakeText(_panelRoot.transform, "INVENTORY", 18f, FontStyles.Bold, Color.white,
                 new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f),
                 new Vector2(8f, -10f), new Vector2(320f, 28f));

        // Close hint
        MakeText(_panelRoot.transform, "[I] Close", 11f, FontStyles.Normal,
                 new Color(0.6f, 0.6f, 0.6f),
                 new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f),
                 new Vector2(-8f, -10f), new Vector2(80f, 20f));

        AddDivider(_panelRoot.transform, new Vector2(0f, -40f));

        // Wallet section
        MakeText(_panelRoot.transform, "WALLET", 12f, FontStyles.Bold,
                 new Color(0.6f, 0.9f, 0.6f),
                 new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f),
                 new Vector2(8f, -50f), new Vector2(300f, 20f));

        var walletValueGO = MakeText(_panelRoot.transform, "$0.00", 14f, FontStyles.Bold,
                 Color.white,
                 new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f),
                 new Vector2(8f, -72f), new Vector2(300f, 22f));
        _walletText = walletValueGO.GetComponent<TextMeshProUGUI>();

        AddDivider(_panelRoot.transform, new Vector2(0f, -102f));

        // Items section header
        MakeText(_panelRoot.transform, "ITEMS", 12f, FontStyles.Bold,
                 new Color(0.6f, 0.9f, 0.6f),
                 new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f),
                 new Vector2(8f, -112f), new Vector2(300f, 20f));

        // Replace everything from "// Scroll view" comment to end of BuildPanelUI with this:
        GameObject content = new GameObject("Content", typeof(RectTransform));
        content.transform.SetParent(_panelRoot.transform, false);
        RectTransform crt = content.GetComponent<RectTransform>();
        crt.anchorMin = new Vector2(0f, 0f);
        crt.anchorMax = new Vector2(1f, 1f);
        crt.offsetMin = new Vector2(6f, 8f);
        crt.offsetMax = new Vector2(-6f, -136f);

        VerticalLayoutGroup vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.childAlignment        = TextAnchor.UpperLeft;
        vlg.childControlHeight    = true;
        vlg.childControlWidth     = true;
        vlg.childForceExpandWidth  = true;
        vlg.childForceExpandHeight = false;
        vlg.spacing = 2f;
        vlg.padding = new RectOffset(4, 4, 4, 4);

        _itemListParent = content.transform;
    }
    // ── Helpers ────────────────────────────────────────────────────────
    GameObject MakeText(Transform parent, string text, float size, FontStyles style,
                        Color color, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
                        Vector2 anchoredPos, Vector2 sizeDelta)
    {
        GameObject go = new GameObject(text.Length > 12 ? text[..12] : text, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin        = anchorMin;
        rt.anchorMax        = anchorMax;
        rt.pivot            = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta        = sizeDelta;

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text      = text;
        tmp.fontSize  = size;
        tmp.fontStyle = style;
        tmp.color     = color;
        return go;
    }

    void AddDivider(Transform parent, Vector2 anchoredPos)
    {
        GameObject div = new GameObject("Divider", typeof(RectTransform));
        div.transform.SetParent(parent, false);
        RectTransform rt = div.GetComponent<RectTransform>();
        rt.anchorMin        = new Vector2(0f, 1f);
        rt.anchorMax        = new Vector2(1f, 1f);
        rt.pivot            = new Vector2(0.5f, 1f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta        = new Vector2(-16f, 1f);
        div.AddComponent<Image>().color = new Color(0.3f, 0.3f, 0.35f, 1f);
    }
}
