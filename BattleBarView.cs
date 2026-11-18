using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Верхний боевой бар. Создаёт свой Canvas автоматически, поэтому не требует ручной сборки сцены.
/// Раскладка: [Иконка P1][Имя+HP P1] ......... [Имя+HP P2][Иконка P2]
/// Сетка GridView позиционируется НИЖЕ этого бара (см. GameBootstrap).
/// </summary>
public sealed class BattleBarView : MonoBehaviour
{
    [Header("Данные")]
    public string PlayerName = "Hero";
    public string EnemyName = "Goblin";
    public int MaxHp = 30;

    [Header("Визуал")]
    public Color PlayerColor = new Color(0.2f, 0.7f, 1f);
    public Color EnemyColor = new Color(1f, 0.35f, 0.3f);
    public float BarHeightPx = 90f;      // высота самого бара
    public Vector2 IconSize = new Vector2(64f, 64f);
    public Vector2 BarSize = new Vector2(180f, 18f);

    private Image _playerFill, _enemyFill;
    private Text _playerText, _enemyText;
    private int _playerHp, _enemyHp;

    public RectTransform RootRect { get; private set; } // чтобы Bootstrap прижал сетку под бар

    private void Awake()
    {
        EnsureCanvas();
        Build();
        SetHp(MaxHp, MaxHp);
    }

    private void EnsureCanvas()
    {
        if (FindObjectOfType<Canvas>() != null) return;

        var canvasGO = new GameObject("BattleCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // поверх игры (Клетки=0, Подсветка=1, Фигуры=2 — это sortingOrder в мире)
        canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGO.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1080f, 1920f);
        canvasGO.AddComponent<GraphicRaycaster>();
    }

    private void Build()
    {
        var canvas = FindObjectOfType<Canvas>();
        var barGO = new GameObject("BattleBar", typeof(RectTransform));
        barGO.transform.SetParent(canvas.transform, false);

        RootRect = (RectTransform)barGO.transform;
        RootRect.anchorMin = new Vector2(0f, 1f);
        RootRect.anchorMax = new Vector2(1f, 1f);
        RootRect.pivot = new Vector2(0.5f, 1f);
        RootRect.sizeDelta = new Vector2(0f, BarHeightPx);
        RootRect.anchoredPosition = Vector2.zero;

        var bg = barGO.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.55f);
        bg.raycastTarget = false;

        // --- Левая сторона: игрок ---
        var pIcon = CreateImage(barGO.transform, "P1_Icon", PlayerColor);
        LayoutSide(pIcon.rectTransform, left: true, isIcon: true);

        _playerFill = CreateBar(barGO.transform, "P1_HpBg", PlayerColor, out var pFillTf);
        _playerFill = pFillTf;
        LayoutSide(_playerFill.transform.parent, left: true, isIcon: false);

        _playerText = CreateText(barGO.transform, "P1_Name", PlayerName);
        LayoutSide(_playerText.rectTransform, left: true, isIcon: false, nameOffset: true);

        // --- Правая сторона: враг ---
        var eIcon = CreateImage(barGO.transform, "P2_Icon", EnemyColor);
        LayoutSide(eIcon.rectTransform, left: false, isIcon: true);

        _enemyFill = CreateBar(barGO.transform, "P2_HpBg", EnemyColor, out var eFillTf);
        _enemyFill = eFillTf;
        LayoutSide(_enemyFill.transform.parent, left: false, isIcon: false);

        _enemyText = CreateText(barGO.transform, "P2_Name", EnemyName);
        LayoutSide(_enemyText.rectTransform, left: false, isIcon: false, nameOffset: true);
    }

    // Простая ручная раскладка без HorizontalLayoutGroup — меньше GC на кадрах перестроения
    private void LayoutSide(RectTransform rt, bool left, bool isIcon, bool nameOffset = false)
    {
        const float margin = 12f;
        float xDir = left ? 1f : -1f;
        float xOffset = margin + (isIcon ? IconSize.x * 0.5f : IconSize.x + BarSize.x * 0.5f * 0f + 12f + BarSize.x * 0.5f);
        if (nameOffset) xOffset = margin + IconSize.x + 12f + BarSize.x * 0.5f;

        rt.anchorMin = rt.anchorMax = left ? new Vector2(0f, 0.5f) : new Vector2(1f, 0.5f);
        rt.pivot = new Vector2(left ? 0f : 1f, 0.5f);
        rt.sizeDelta = isIcon ? IconSize : (nameOffset ? new Vector2(160f, 20f) : BarSize);
        rt.anchoredPosition = new Vector2(xDir * (margin + (isIcon ? 0f : IconSize.x + 12f + (nameOffset ? 0f : 0f))),
                                          isIcon ? 0f : (nameOffset ? 22f : -14f));
        if (!left) rt.pivot = new Vector2(1f, 0.5f);
    }

    private Image CreateImage(Transform parent, string name, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = color;
        img.raycastTarget = false;
        return img;
    }

    private Image CreateBar(Transform parent, string name, Color fill, out Image fillImg)
    {
        var bg = CreateImage(parent, name, new Color(0.15f, 0.15f, 0.15f, 0.9f));
        var f = CreateImage(bg.transform, "Fill", fill);
        var fRt = f.rectTransform;
        fRt.anchorMin = Vector2.zero;
        fRt.anchorMax = Vector2.one;
        fRt.offsetMin = new Vector2(2f, 2f);   // рамка 2px
        fRt.offsetMax = new Vector2(-2f, -2f);
        f.type = Image.Type.Filled;
        f.fillMethod = Image.FillMethod.Horizontal;
        f.fillOrigin = (int)Image.OriginHorizontal.Left;
        fillImg = f;
        return bg;
    }

    private Text CreateText(Transform parent, string name, string content)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var t = go.AddComponent<Text>();
        t.text = content;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = 18;
        t.alignment = TextAnchor.MiddleLeft;
        t.color = Color.white;
        t.raycastTarget = false;
        return t;
    }

    /// <summary>Публичный API для бой-логики. Вызывается только при изменении HP, НЕ в Update.</summary>
    public void SetHp(int playerHp, int enemyHp)
    {
        _playerHp = Mathf.Clamp(playerHp, 0, MaxHp);
        _enemyHp = Mathf.Clamp(enemyHp, 0, MaxHp);
        _playerFill.fillAmount = (float)_playerHp / MaxHp;
        _enemyFill.fillAmount = (float)_enemyHp / MaxHp;
        _playerText.text = $"{PlayerName} {_playerHp}/{MaxHp}";
        _enemyText.text = $"{_enemyHp}/{MaxHp} {EnemyName}";
    }

    public void DamageEnemy(int dmg) => SetHp(_playerHp, _enemyHp - dmg);
    public void DamagePlayer(int dmg) => SetHp(_playerHp - dmg, _enemyHp);
    public bool IsBattleOver => _playerHp <= 0 || _enemyHp <= 0;
}