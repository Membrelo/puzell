using UnityEngine;

public class InputController : MonoBehaviour
{
    public GameBootstrap Bootstrap;
    public HighlightView HighlightView;

    private Piece _draggedPiece;
    private Piece _hoveredPiece;

    void Update()
    {
        Vector2Int mouseCell = GetMouseCell();

        if (Input.GetMouseButtonDown(0))
        {
            _draggedPiece = GetPieceAt(mouseCell);
            if (_draggedPiece != null)
            {
                HighlightView.Show(Bootstrap.State, _draggedPiece);
            }
        }

        if (Input.GetMouseButtonUp(0) && _draggedPiece != null)
        {
            TryShiftToMouse(mouseCell, _draggedPiece);
            HighlightView.HideAll();
            _draggedPiece = null;
        }

        if (Input.GetMouseButtonDown(1))
        {
            _hoveredPiece = GetPieceAt(mouseCell);
        }

        if (Input.GetMouseButtonUp(1))
        {
            _hoveredPiece = null;
        }
    }

    private void TryShiftToMouse(Vector2Int mouseCell, Piece piece)
    {
        // Вычисляем центр фигуры
        float centerX = piece.Anchor.x + (piece.Shape.Width / 2f);
        float centerY = piece.Anchor.y + (piece.Shape.Height / 2f);

        // Разница между мышкой и центром
        float dx = mouseCell.x - centerX;
        float dy = mouseCell.y - centerY;

        // Если мышь не сдвинулась дальше половины клетки, отменяем ход
        if (Mathf.Abs(dx) < 0.3f && Mathf.Abs(dy) < 0.3f) return; 

        ShiftDirection dir;
        // Определяем, куда именно потянули мышь
        if (Mathf.Abs(dx) > Mathf.Abs(dy))
            dir = dx > 0 ? ShiftDirection.Right : ShiftDirection.Left;
        else
            dir = dy > 0 ? ShiftDirection.Up : ShiftDirection.Down;

        if (ShiftResolver.TryShift(Bootstrap.State, piece, dir))
        {
            Bootstrap.RefreshVisuals();
        }
    }

    private Piece GetPieceAt(Vector2Int cell)
    {
        if (Bootstrap.State.InBounds(cell))
        {
            int? pieceId = Bootstrap.State.Grid[cell.x, cell.y].PieceId;
            if (pieceId.HasValue)
            {
                return Bootstrap.State.GetPieceById(pieceId.Value);
            }
        }
        return null;
    }

    private Vector2Int GetMouseCell()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2Int(Mathf.FloorToInt(mouseWorld.x), Mathf.FloorToInt(mouseWorld.y));
    }

    private void OnGUI()
    {
        if (_hoveredPiece != null)
        {
            Vector3 mousePos = Input.mousePosition;
            float width = 200;
            float height = 60;
            float padding = 15;

            float x = mousePos.x - width - padding;
            float y = (Screen.height - mousePos.y) - height - padding;

            if (x < 0) x = mousePos.x + padding; 
            if (y < 0) y = (Screen.height - mousePos.y) + padding;

            Rect rect = new Rect(x, y, width, height);
            GUI.color = new Color(0, 0, 0, 0.8f);
            GUI.Box(rect, "");
            GUI.color = Color.white;
            
            GUI.Label(new Rect(rect.x + 10, rect.y + 10, rect.width - 20, rect.height - 20), 
                $"Тип: {_hoveredPiece.Type}\nУрон: {_hoveredPiece.Area}");
        }
    }
}