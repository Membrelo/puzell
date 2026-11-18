using System.Collections.Generic;
using UnityEngine;

public class HighlightView : MonoBehaviour
{
    private List<GameObject> _ghosts = new List<GameObject>();
    private System.Func<Piece, Sprite> _getSprite;
    private ShiftDirection[] _directions = { ShiftDirection.Up, ShiftDirection.Down, ShiftDirection.Left, ShiftDirection.Right };

    public void Init(System.Func<Piece, Sprite> getSprite)
    {
        _getSprite = getSprite;
    }

    public void Show(GameState state, Piece piece)
    {
        HideAll();

        foreach (var dir in _directions)
        {
            var delta = ShiftResolver.DeltaOf(dir);
            var targetAnchor = piece.Anchor + delta;

            if (state.CanPlace(piece, targetAnchor))
            {
                var ghostContainer = new GameObject($"GhostContainer_{dir}");
                ghostContainer.transform.SetParent(transform);
                // Контейнер ставим в левый нижний угол новой позиции
                ghostContainer.transform.position = new Vector3(targetAnchor.x, targetAnchor.y, 0);

                var spriteGo = new GameObject("Sprite");
                spriteGo.transform.SetParent(ghostContainer.transform);
                
                // Сдвигаем картинку на полклетки вправо и вверх
                spriteGo.transform.localPosition = new Vector3(piece.Shape.Width / 2f, piece.Shape.Height / 2f, 0);

                var sr = spriteGo.AddComponent<SpriteRenderer>();
                sr.sprite = _getSprite(piece); 
                sr.color = new Color(0f, 1f, 0f, 0.4f); 
                sr.sortingOrder = 1; 

                float scaleX = (piece.Shape.Width * 0.95f) / sr.sprite.bounds.size.x;
                float scaleY = (piece.Shape.Height * 0.95f) / sr.sprite.bounds.size.y;
                spriteGo.transform.localScale = new Vector3(scaleX, scaleY, 1f);

                _ghosts.Add(ghostContainer);
            }
        }
    }

    public void HideAll()
    {
        foreach (var ghost in _ghosts) Destroy(ghost);
        _ghosts.Clear();
    }
}