using System.Collections.Generic;
using Core.Managers;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryWalletPanel : MonoBehaviour
{
    private GameObject       _panelRoot;
    private bool             _isOpen = false;
    private TextMeshProUGUI  _walletText;
    private Transform        _itemListParent;

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

    void TogglePanel()
    {
        _isOpen = !_isOpen;
        _panelRoot.SetActive(_isOpen);
        if (_isOpen) RefreshContent();
    }

    void RefreshContent()
    {
        if (_walletText != null && PlayerWallet.Instance != null)
            _walletText.text = $"Balance:  ${PlayerWallet.Instance.balance:F2}";

        if (_itemListParent == null || InventoryManager.Instance == null) return;

        for (int i = _itemListParent.childCount - 1; i >= 0; i--)
            DestroyImmediate(_itemListParent.GetChild(i).gameObject);

        List<InventoryManager.InventoryItem> items = InventoryManager.Instance.items;

        if (items.Count == 0)
        {
            AddRow(_itemListParent, "  (empty)", Color.gray, null);
        }
        else
        {
            foreach (var item in items)
            {
                if (item.itemKind == InventoryItemKind.CookedMeal)
                {
                    string label = $"[Meal]  {item.mealName} — {item.qualityTier}";
                    // Capture for lambda
                    var capturedItem = item;
                    AddRow(_itemListParent, label, new Color(1f, 0.85f, 0.4f), () =>
                    {
                        InventoryManager.Instance.EatMeal(capturedItem);
                        RefreshContent();
                    });
                }
                else
                {
                    string label = $"[Ing]  {item.productName}";
                    AddRow(_itemListParent, label, Color.white, null);
                }
            }
        }

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_panelRoot.GetComponent<RectTransform>());
    }

    // Updated AddRow — pass null for eatAction on ingredients
    void AddRow(Transform parent, string text, Color color, System.Action eatAction)
    {
        GameObject row = new GameObject("Row", typeof(RectTransform));
        row.transform.SetParent(parent, false);

        LayoutElement le = row.AddComponent<LayoutElement>();

        if (eatAction != null)
        {
            // Meal row: taller to fit the Eat button
            le.minHeight       = 30f;
            le.preferredHeight = 30f;
            le.flexibleHeight  = 0f;  // <-- add this to prevent stretching

            HorizontalLayoutGroup hlg = row.AddComponent<HorizontalLayoutGroup>();
            hlg.childAlignment         = TextAnchor.MiddleLeft;
            hlg.childControlHeight     = true;
            hlg.childForceExpandHeight = false; // <-- was true
            hlg.spacing                = 6f;

            // Label
            GameObject labelGO = new GameObject("Label", typeof(RectTransform));
            labelGO.transform.SetParent(row.transform, false);
            LayoutElement labelLE = labelGO.AddComponent<LayoutElement>();
            labelLE.flexibleWidth = 1f;

            TextMeshProUGUI tmp = labelGO.AddComponent<TextMeshProUGUI>();
            tmp.text               = text;
            tmp.fontSize           = 13f;
            tmp.color              = color;
            tmp.enableWordWrapping = false;

            // Eat button
            GameObject btnGO = new GameObject("EatBtn", typeof(RectTransform));
            btnGO.transform.SetParent(row.transform, false);
            LayoutElement btnLE = btnGO.AddComponent<LayoutElement>();
            btnLE.minWidth       = 54f;
            btnLE.preferredWidth = 54f;
            btnLE.minHeight      = 24f;      // <-- add this
            btnLE.preferredHeight = 24f;     // <-- add this

            Image btnImg = btnGO.AddComponent<Image>();
            btnImg.color = new Color(0.25f, 0.6f, 0.3f, 1f);

            Button btn = btnGO.AddComponent<Button>();
            ColorBlock cb = btn.colors;
            cb.highlightedColor = new Color(0.3f, 0.75f, 0.38f, 1f);
            cb.pressedColor     = new Color(0.18f, 0.45f, 0.22f, 1f);
            btn.colors          = cb;
            btn.onClick.AddListener(() => eatAction());

            GameObject btnTextGO = new GameObject("BtnLabel", typeof(RectTransform));
            btnTextGO.transform.SetParent(btnGO.transform, false);
            RectTransform btnTextRT = btnTextGO.GetComponent<RectTransform>();
            btnTextRT.anchorMin        = Vector2.zero;
            btnTextRT.anchorMax        = Vector2.one;
            btnTextRT.offsetMin        = Vector2.zero;
            btnTextRT.offsetMax        = Vector2.zero;

            TextMeshProUGUI btnTMP = btnTextGO.AddComponent<TextMeshProUGUI>();
            btnTMP.text                = "Eat";
            btnTMP.fontSize            = 11f;
            btnTMP.color               = Color.white;
            btnTMP.alignment           = TextAlignmentOptions.Center;
            btnTMP.enableWordWrapping  = false;
        }
        else
        {
            // Plain ingredient row — unchanged behaviour
            le.minHeight       = 22f;
            le.preferredHeight = 22f;
            

            TextMeshProUGUI tmp = row.AddComponent<TextMeshProUGUI>();
            tmp.text               = text;
            tmp.fontSize           = 13f;
            tmp.color              = color;
            tmp.enableWordWrapping = false;
        }
    }

    // ── UI builder — unchanged from your original ──────────────────────
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

        _panelRoot = new GameObject("InventoryWalletPanel", typeof(RectTransform));
        _panelRoot.transform.SetParent(transform, false);
        RectTransform pr = _panelRoot.GetComponent<RectTransform>();
        pr.anchorMin        = new Vector2(0.5f, 0.5f);
        pr.anchorMax        = new Vector2(0.5f, 0.5f);
        pr.pivot            = new Vector2(0.5f, 0.5f);
        pr.sizeDelta        = new Vector2(340f, 460f);
        pr.anchoredPosition = Vector2.zero;

        _panelRoot.AddComponent<Image>().color = new Color(0.05f, 0.05f, 0.1f, 0.92f);

        MakeText(_panelRoot.transform, "INVENTORY", 18f, FontStyles.Bold, Color.white,
                 new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f),
                 new Vector2(8f, -10f), new Vector2(320f, 28f));

        MakeText(_panelRoot.transform, "[I] Close", 11f, FontStyles.Normal,
                 new Color(0.6f, 0.6f, 0.6f),
                 new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f),
                 new Vector2(-8f, -10f), new Vector2(80f, 20f));

        AddDivider(_panelRoot.transform, new Vector2(0f, -40f));

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

        MakeText(_panelRoot.transform, "ITEMS", 12f, FontStyles.Bold,
                 new Color(0.6f, 0.9f, 0.6f),
                 new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f),
                 new Vector2(8f, -112f), new Vector2(300f, 20f));

        GameObject content = new GameObject("Content", typeof(RectTransform));
        content.transform.SetParent(_panelRoot.transform, false);
        RectTransform crt = content.GetComponent<RectTransform>();
        crt.anchorMin = new Vector2(0f, 0f);
        crt.anchorMax = new Vector2(1f, 1f);
        crt.offsetMin = new Vector2(6f, 8f);
        crt.offsetMax = new Vector2(-6f, -136f);

        VerticalLayoutGroup vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.childAlignment         = TextAnchor.UpperLeft;
        vlg.childControlHeight     = true;
        vlg.childControlWidth      = true;
        vlg.childForceExpandWidth  = true;
        vlg.childForceExpandHeight = false;
        vlg.spacing                = 2f;
        vlg.padding                = new RectOffset(4, 4, 4, 4);

        _itemListParent = content.transform;
    }

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