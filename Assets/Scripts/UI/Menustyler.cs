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
    public Color panelColor      = new Color(0f, 0.12f, 0.25f, 0.85f);
    public Color buttonNormal    = new Color(0f, 0.6f,  0.9f,  0.25f);
    public Color buttonHighlight = new Color(0f, 0.9f,  1f,    0.5f);
    public Color buttonBorder    = new Color(0f, 0.8f,  1f,    0.9f);
    public Color titleColor      = new Color(0f, 1f,    1f,    1f);
    public Color subtitleColor   = new Color(0.6f, 0.9f, 1f,  0.7f);

    [Header("Animación")]
    public float floatSpeed     = 0.8f;
    public float floatAmount    = 0.03f;
    public float scanlineSpeed  = 120f;
    public float glitchInterval = 5f;

    // Internos
    private CanvasGroup canvasGroup;
    private RectTransform scanline;
    private float scanlineY;
    private float panelH;
    private Vector3 startPos;
    private Image[] buttonImages;
    private bool[] buttonHovered;

    void Start()
    {
        startPos = transform.localPosition;

        SetupCanvasGroup();
        StylePanel();
        StyleTitle();
        StyleButtons();
        CreateScanline();
        CreateBorders();
        CreateCornerDecorations();

        StartCoroutine(AnimateEntrance());
        StartCoroutine(GlitchLoop());
    }

    void Update()
    {
        AnimateFloat();
        MoveScanline();
        PulseButtons();
    }

    // ── Setup ──
    void SetupCanvasGroup()
    {
        canvasGroup = mainPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = mainPanel.gameObject.AddComponent<CanvasGroup>();
    }

    void StylePanel()
    {
        Image img = mainPanel.GetComponent<Image>();
        if (img == null) img = mainPanel.gameObject.AddComponent<Image>();
        img.color = panelColor;
        panelH = mainPanel.rect.height;
    }

    void StyleTitle()
    {
        if (titleText != null)
        {
            titleText.color    = titleColor;
            titleText.fontSize = 32;
            titleText.fontStyle = FontStyles.Bold;
        }
        if (subtitleText != null)
        {
            subtitleText.color    = subtitleColor;
            subtitleText.fontSize = 11;
        }
    }

    void StyleButtons()
    {
        if (menuButtons == null) return;
        buttonImages  = new Image[menuButtons.Length];
        buttonHovered = new bool[menuButtons.Length];

        for (int i = 0; i < menuButtons.Length; i++)
        {
            Button btn = menuButtons[i];
            if (btn == null) continue;

            // Fondo del botón
            Image img = btn.GetComponent<Image>();
            if (img == null) img = btn.gameObject.AddComponent<Image>();
            img.color = buttonNormal;
            buttonImages[i] = img;

            // Texto del botón
            TextMeshProUGUI tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.color    = new Color(0.8f, 1f, 1f, 1f);
                tmp.fontSize = 13;
                tmp.fontStyle = FontStyles.Bold;
                tmp.alignment = TextAlignmentOptions.Center;
            }

            // Colores de transición
            ColorBlock cb = btn.colors;
            cb.normalColor      = buttonNormal;
            cb.highlightedColor = buttonHighlight;
            cb.pressedColor     = new Color(0f, 1f, 0.6f, 0.6f);
            cb.selectedColor    = buttonHighlight;
            btn.colors = cb;

            // Borde del botón
            CreateButtonBorder(btn.GetComponent<RectTransform>());
        }
    }

    void CreateButtonBorder(RectTransform btnRect)
    {
        if (btnRect == null) return;

        GameObject border = new GameObject("Border");
        border.transform.SetParent(btnRect, false);

        RectTransform rt = border.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Outline outline = border.AddComponent<Outline>();
        outline.effectColor    = buttonBorder;
        outline.effectDistance = new Vector2(1f, 1f);
    }

    void CreateScanline()
    {
        GameObject sl = new GameObject("Scanline");
        sl.transform.SetParent(mainPanel, false);

        RectTransform rt = sl.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(mainPanel.rect.width, 2f);

        Image img = sl.AddComponent<Image>();
        img.color = new Color(0f, 1f, 1f, 0.12f);

        scanline  = rt;
        scanlineY = -panelH / 2f;
    }

    void CreateBorders()
    {
        string[] names = { "BorderTop","BorderBottom","BorderLeft","BorderRight" };
        float w = mainPanel.rect.width;
        float h = mainPanel.rect.height;

        (Vector2 size, Vector2 pos)[] configs = {
            (new Vector2(w,  1.5f), new Vector2(0,  h/2f)),
            (new Vector2(w,  1.5f), new Vector2(0, -h/2f)),
            (new Vector2(1.5f, h), new Vector2(-w/2f, 0)),
            (new Vector2(1.5f, h), new Vector2( w/2f, 0)),
        };

        for (int i = 0; i < 4; i++)
        {
            GameObject b = new GameObject(names[i]);
            b.transform.SetParent(mainPanel, false);

            RectTransform rt = b.AddComponent<RectTransform>();
            rt.sizeDelta        = configs[i].size;
            rt.anchoredPosition = configs[i].pos;

            Image img = b.AddComponent<Image>();
            img.color = new Color(0f, 0.85f, 1f, 0.6f);
        }
    }

    void CreateCornerDecorations()
    {
        float w = mainPanel.rect.width  / 2f - 5f;
        float h = mainPanel.rect.height / 2f - 5f;
        float s = 14f;

        Vector2[] corners = {
            new Vector2(-w,  h), new Vector2( w,  h),
            new Vector2(-w, -h), new Vector2( w, -h)
        };

        foreach (var c in corners)
        {
            // Horizontal
            CreateCornerLine(c, new Vector2(s, 2f));
            // Vertical
            CreateCornerLine(c, new Vector2(2f, s));
        }
    }

    void CreateCornerLine(Vector2 pos, Vector2 size)
    {
        GameObject obj = new GameObject("CornerDeco");
        obj.transform.SetParent(mainPanel, false);

        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.sizeDelta        = size;
        rt.anchoredPosition = pos;

        Image img = obj.AddComponent<Image>();
        img.color = new Color(0f, 1f, 1f, 0.9f);
    }

    // ── Animaciones ──
    void AnimateFloat()
    {
        float y = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        transform.localPosition = new Vector3(startPos.x, y, startPos.z);
    }

    void MoveScanline()
    {
        if (scanline == null) return;
        scanlineY += scanlineSpeed * Time.deltaTime;
        if (scanlineY > panelH / 2f) scanlineY = -panelH / 2f;
        scanline.anchoredPosition = new Vector2(0, scanlineY);
    }

    void PulseButtons()
    {
        if (buttonImages == null) return;
        float pulse = (Mathf.Sin(Time.time * 2f) + 1f) / 2f;

        for (int i = 0; i < buttonImages.Length; i++)
        {
            if (buttonImages[i] == null) continue;
            Color c = buttonNormal;
            c.a = 0.2f + pulse * 0.1f;
            buttonImages[i].color = c;
        }
    }

    IEnumerator AnimateEntrance()
    {
        canvasGroup.alpha = 0f;
        transform.localScale = Vector3.one * 0.85f;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.7f;
            float s = t * t * (3f - 2f * t);
            canvasGroup.alpha = s;
            transform.localScale = Vector3.Lerp(Vector3.one * 0.85f, Vector3.one, s);
            yield return null;
        }
        canvasGroup.alpha = 1f;
        transform.localScale = Vector3.one;
    }

    IEnumerator GlitchLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(glitchInterval + Random.Range(-1.5f, 1.5f));

            for (int i = 0; i < 3; i++)
            {
                canvasGroup.alpha = Random.Range(0.55f, 0.9f);
                yield return new WaitForSeconds(0.04f);
                canvasGroup.alpha = 1f;
                yield return new WaitForSeconds(0.03f);
            }
        }
    }
}