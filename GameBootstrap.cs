using UnityEngine;

public sealed class GameBootstrap : MonoBehaviour
{
    public const int GridW = 5;
    public const int GridH = 5;
    public const float CellSize = 1f;

    [SerializeField] private Sprite cellSprite;   // белый квадрат (pivot = Center)
    [SerializeField] private Sprite pieceSprite;  // спрайт клетки фигуры

    public static GameBootstrap Instance { get; private set; }

    public GameState State { get; private set; }
    public GridView GridView { get; private set; }
    public BattleBarView BattleBar { get; private set; }
    public HighlightView Highlights { get; private set; }

    private void Awake()
    {
        Instance = this;

        State = new GameState(GridW, GridH);

        // Слой 0: Клетки
        GridView = new GameObject("GridView").AddComponent<GridView>();
        GridView.Init(State, cellSprite, CellSize);

        // Слой 1: Подсветка
        Highlights = new GameObject("HighlightView").AddComponent<HighlightView>();
        Highlights.Init(GridView, cellSprite);

        // Слой 2: Фигуры
        var piecesRoot = new GameObject("PiecesRoot");
        PieceViewPool.Init(piecesRoot.transform, pieceSprite, GridView, CellSize);

        // Верхний боевой бар: слева игрок, справа враг (иконки + имена + HP-полоски)
        BattleBar = new GameObject("BattleBar").AddComponent<BattleBarView>();
        BattleBar.Init();

        SpawnInitialPieces();
    }

    private void Start()
    {
        // Стартовый спавн: теперь фигуры НИКОГДА не встают "поперёк" или за края сетки —
        // SpawnRandomPiece проверяет CanPlace для каждой candidate-позиции.
        for (int i = 0; i < 3; i++)
            State.SpawnRandomPiece(PieceType.Normal, out _);

        RefreshAllViews();
    }

    public void RefreshAllViews()
    {
        PieceViewPool.SyncAll(State.Pieces);
    }

    /// <summary>Ход завершён: сдвигаем/спавним, обновляем бар.</summary>
    public void OnTurnCommitted()
    {
        State.SpawnRandomPiece(PieceType.Normal, out bool spawned);
        RefreshAllViews();

        if (!spawned || State.IsGameOver())
            Debug.Log("[Game] No valid placements — turn skipped / game over check.");
    }
}