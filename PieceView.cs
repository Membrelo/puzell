using System.Collections.Generic;
using UnityEngine;

public class PieceView : MonoBehaviour
{
    private GameState _state;
    private Dictionary<int, GameObject> _pieceObjects = new Dictionary<int, GameObject>();
    private System.Func<Piece, Sprite> _getSprite;

    public void Init(GameState state, System.Func<Piece, Sprite> getSprite)
    {
        _state = state;
        _getSprite = getSprite;
        _pieceObjects.Clear();

        foreach (var piece in _state.Pieces)
        {
            // Контейнер будет стоять в левом нижнем углу (Anchor)
            var container = new GameObject($"PieceContainer_{piece.Id}");
            container.transform.SetParent(transform);

            var spriteGo = new GameObject($"Sprite_{piece.Id}");
            spriteGo.transform.SetParent(container.transform);
            
            // МАГИЯ ЗДЕСЬ: Сдвигаем картинку на полклетки вправо и вверх.
            // Так как Anchor — это левый нижний угол, а картинка центрируется, 
            // нам нужно сместить её на половину её размера.
            spriteGo.transform.localPosition = new Vector3(piece.Shape.Width / 2f, piece.Shape.Height / 2f, 0);

            var sr = spriteGo.AddComponent<SpriteRenderer>();
            sr.sprite = _getSprite(piece); 
            sr.color = GetPieceColor(piece.Type); 
            sr.sortingOrder = 2;

            // Растягиваем под размер фигуры
            float scaleX = (piece.Shape.Width * 0.95f) / sr.sprite.bounds.size.x;
            float scaleY = (piece.Shape.Height * 0.95f) / sr.sprite.bounds.size.y;
            spriteGo.transform.localScale = new Vector3(scaleX, scaleY, 1f);

            _pieceObjects[piece.Id] = container;
        }
        Refresh();
    }

    public void Refresh()
    {
        foreach (var piece in _state.Pieces)
        {
            if (_pieceObjects.TryGetValue(piece.Id, out var container))
            {
                // Ставим контейнер РОВНО в координаты Anchor (левый нижний угол)
                // Без вычисления центров!
                container.transform.position = new Vector3(piece.Anchor.x, piece.Anchor.y, 0);
            }
        }
    }

    private Color GetPieceColor(PieceType type)
    {
        switch (type)
        {
            case PieceType.Heal:   return new Color(0.7f, 1f, 0.7f); 
            case PieceType.Sharp:  return new Color(1f, 0.7f, 0.7f); 
            case PieceType.Ice:    return new Color(0.8f, 0.9f, 1f); 
            case PieceType.Bouncy: return new Color(1f, 0.8f, 1f); 
            case PieceType.Echo:   return new Color(1f, 1f, 0.7f); 
            default:               return Color.white; 
        }
    }
}