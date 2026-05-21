using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MenuStyler : MonoBehaviour
{
    [Header("Referencias")]
    public RectTransform mainPanel;
    public Button[] menuButtons;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subtitleText;

    [Header("Colores")]
    public Color panelColor      = new Color(0f, 0.08f, 0.18f, 0.88f);
    public Color buttonNormal    = new Color(0f, 0.5f,  0.85f, 0.28f);
    public Color buttonHighlight = new Color(0f, 0.85f, 1f,    0.5f);
    public Color titleColor      = new Color(0f, 1f,    1f,    1f);
    public Color subtitleColor   = new Color(0.6f, 0.9f, 1f,   0.75f);

    [Header("Animación")]
    public float floatSpeed     = 0.8f;
    public float floatAmount    = 0.025f;
    public float scanlineSpeed  = 100f;
    public float glitchInterval = 6f;

    private CanvasGroup canvasGroup;
    private RectTransform scanline;
    private float scanlineY;
    private float panelH;
    private Vector3 startLocalPos;
    private Image[] buttonImages;

    void Start()
    {
        startLocalPos = transform.localPosition;

        SetupCanvasGroup();
        StylePanel();
        StyleTitle();
        StyleButtons();
        CreateScanline();
        CreateBorders();
        CreateCornerDecorations();
        StartCoroutine(GlitchLoop());
    }

    void Update()
    {
        AnimateFloat();
        MoveScanline();
        PulseButtons();
    }

    void SetupCanvasGroup()
    {
        canvasGroup = mainPanel != null
            ? mainPanel.GetComponent<CanvasGroup>()
            : GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = (mainPanel != null ? mainPanel.gameObject : gameObject)
                          .AddComponent<CanvasGroup>();

        // SIEMPRE visible al inicio — sin animación de entrada
        canvasGroup.alpha          = 1f;
        canvasGroup.interactable   = true;
        canvasGroup.blocksRaycasts = true;
    }

    void StylePanel()
    {
        if (mainPanel == null) return;
        Image img = mainPanel.GetComponent<Image>();
        if (img == null) img = mainPanel.gameObject.AddComponent<Image>();
        img.color = panelColor;
        panelH    = mainPanel.rect.height;
    }

    void StyleTitle()
    {
        if (titleText    != null) { titleText.color    = titleColor;    titleText.fontSize    = 30; titleText.fontStyle = FontStyles.Bold; }
        if (subtitleText != null) { subtitleText.color = subtitleColor; subtitleText.fontSize = 11; }
    }

    void StyleButtons()
    {
        if (menuButtons == null) return;
        buttonImages = new Image[menuButtons.Length];

        for (int i = 0; i < menuButtons.Length; i++)
        {
            if (menuButtons[i] == null) continue;

            Image img = menuButtons[i].GetComponent<Image>();
            if (img == null) img = menuButtons[i].gameObject.AddComponent<Image>();
            img.color       = buttonNormal;
            buttonImages[i] = img;

            TextMeshProUGUI tmp = menuButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.color     = new Color(0.85f, 1f, 1f, 1f);
                tmp.fontSize  = 13;
                tmp.fontStyle = FontStyles.Bold;
                tmp.alignment = TextAlignmentOptions.Center;
            }

            ColorBlock cb         = menuButtons[i].colors;
            cb.normalColor        = buttonNormal;
            cb.highlightedColor   = buttonHighlight;
            cb.pressedColor       = new Color(0f, 1f, 0.6f, 0.55f);
            cb.selectedColor      = buttonHighlight;
            menuButtons[i].colors = cb;
        }
    }

    void CreateScanline()
    {
        if (mainPanel == null) return;
        GameObject sl   = new GameObject("Scanline");
        sl.transform.SetParent(mainPanel, false);
        RectTransform rt = sl.AddComponent<RectTransform>();
        rt.sizeDelta     = new Vector2(mainPanel.rect.width, 2f);
        sl.AddComponent<Image>().color = new Color(0f, 1f, 1f, 0.10f);
        scanline  = rt;
        scanlineY = -(panelH / 2f);
    }

    void CreateBorders()
    {
        if (mainPanel == null) return;
        float w = mainPanel.rect.width;
        float h = mainPanel.rect.height;
        CreateLine("BorderTop",    new Vector2(w, 2f),  new Vector2(0,      h / 2f));
        CreateLine("BorderBottom", new Vector2(w, 2f),  new Vector2(0,     -h / 2f));
        CreateLine("BorderLeft",   new Vector2(2f, h),  new Vector2(-w/2f,  0));
        CreateLine("BorderRight",  new Vector2(2f, h),  new Vector2( w/2f,  0));
    }

    void CreateLine(string name, Vector2 size, Vector2 pos)
    {
        GameObject obj      = new GameObject(name);
        obj.transform.SetParent(mainPanel, false);
        RectTransform rt    = obj.AddComponent<RectTransform>();
        rt.sizeDelta        = size;
        rt.anchoredPosition = pos;
        obj.AddComponent<Image>().color = new Color(0f, 0.85f, 1f, 0.55f);
    }

    void CreateCornerDecorations()
    {
        if (mainPanel == null) return;
        float w = mainPanel.rect.width  / 2f - 5f;
        float h = mainPanel.rect.height / 2f - 5f;
        float s = 14f;
        Vector2[] corners = {
            new Vector2(-w,  h), new Vector2( w,  h),
            new Vector2(-w, -h), new Vector2( w, -h)
        };
        foreach (var c in corners)
        {
            CreateCornerLine(c, new Vector2(s, 2f));
            CreateCornerLine(c, new Vector2(2f, s));
        }
    }

    void CreateCornerLine(Vector2 pos, Vector2 size)
    {
        GameObject obj      = new GameObject("Corner");
        obj.transform.SetParent(mainPanel, false);
        RectTransform rt    = obj.AddComponent<RectTransform>();
        rt.sizeDelta        = size;
        rt.anchoredPosition = pos;
        obj.AddComponent<Image>().color = new Color(0f, 1f, 1f, 0.9f);
    }

    void AnimateFloat()
    {
        float y = startLocalPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        transform.localPosition = new Vector3(startLocalPos.x, y, startLocalPos.z);
    }

    void MoveScanline()
    {
        if (scanline == null || panelH == 0f) return;
        scanlineY += scanlineSpeed * Time.deltaTime;
        if (scanlineY > panelH / 2f) scanlineY = -(panelH / 2f);
        scanline.anchoredPosition = new Vector2(0, scanlineY);
    }

    void PulseButtons()
    {
        if (buttonImages == null) return;
        float pulse = (Mathf.Sin(Time.time * 1.8f) + 1f) / 2f;
        Color c     = buttonNormal;
        c.a         = 0.22f + pulse * 0.1f;
        foreach (var img in buttonImages)
            if (img != null) img.color = c;
    }

    IEnumerator GlitchLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(glitchInterval + Random.Range(-1.5f, 1.5f));
            if (canvasGroup == null) yield break;
            for (int i = 0; i < 3; i++)
            {
                canvasGroup.alpha = Random.Range(0.6f, 0.92f);
                yield return new WaitForSeconds(0.04f);
                canvasGroup.alpha = 1f;
                yield return new WaitForSeconds(0.03f);
            }
            canvasGroup.alpha = 1f;
        }
    }
}