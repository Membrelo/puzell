using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    public GridView GridView;
    public PieceView PieceView;
    public HighlightView HighlightView;

    [Header("Спрайты клеток")]
    public Sprite CellSprite;
    public Sprite CellIceSprite;
    public Sprite CellStickySprite;

    [Header("Спрайты фигур")]
    public Sprite Sprite1x1;
    public Sprite Sprite2x2;

    private GameState _state;
    public GameState State => _state;

    void Start()
    {
        SetupCamera();
        _state = new GameState();

        var p1 = new Piece { Type = PieceType.Normal, Shape = PieceShape.Square2, Anchor = new Vector2Int(1, 1) };
        var p2 = new Piece { Type = PieceType.Ice, Shape = PieceShape.Single, Anchor = new Vector2Int(3, 3) };
        
        _state.PlacePiece(p1);
        _state.PlacePiece(p2);

        _state.Grid[4, 0].Effect = CellEffectType.Ice;
        _state.Grid[0, 4].Effect = CellEffectType.Sticky;

        GridView.Init(_state, CellSprite, CellIceSprite, CellStickySprite);
        PieceView.Init(_state, GetSpriteForPiece);
        HighlightView.Init(GetSpriteForPiece);
    }

    public Sprite GetSpriteForPiece(Piece piece)
    {
        if (piece.Shape == PieceShape.Single)  return Sprite1x1;
        if (piece.Shape == PieceShape.Square2) return Sprite2x2;
        return Sprite1x1; 
    }

    public void RefreshVisuals()
    {
        GridView.Refresh();
        PieceView.Refresh();
    }

    private void SetupCamera()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.transform.position = new Vector3(2.5f, 2.5f, -10f);
            cam.orthographic = true;
            cam.orthographicSize = 2.8f;
            cam.backgroundColor = new Color(0.1f, 0.1f, 0.1f);
        }
    }
}