using UnityEngine;

public class GridView : MonoBehaviour
{
    private GameState _state;
    private SpriteRenderer[,] _cellRenderers;
    private Sprite _spriteNormal, _spriteIce, _spriteSticky;

    public void Init(GameState state, Sprite normal, Sprite ice, Sprite sticky)
    {
        _state = state;
        _spriteNormal = normal;
        _spriteIce = ice;
        _spriteSticky = sticky;
        _cellRenderers = new SpriteRenderer[GameState.Size, GameState.Size];

        for (int x = 0; x < GameState.Size; x++)
        {
            for (int y = 0; y < GameState.Size; y++)
            {
                var go = new GameObject($"Cell_{x}_{y}");
                go.transform.SetParent(transform);
                go.transform.localPosition = new Vector3(x + 0.5f, y + 0.5f, 0);

                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = _spriteNormal;
                _cellRenderers[x, y] = sr;

                // УМНЫЙ МАСШТАБ: Вычисляем, во сколько раз нужно увеличить картинку, 
                // чтобы она стала ровно 1х1 юнит. +2% (1.02f) для наложения, чтобы не было швов.
                float scaleX = 1.02f / sr.sprite.bounds.size.x;
                float scaleY = 1.02f / sr.sprite.bounds.size.y;
                go.transform.localScale = new Vector3(scaleX, scaleY, 1f);
            }
        }
        Refresh();
    }

    public void Refresh()
    {
        for (int x = 0; x < GameState.Size; x++)
        {
            for (int y = 0; y < GameState.Size; y++)
            {
                var cell = _state.Grid[x, y];
                var sr = _cellRenderers[x, y];

                sr.sprite = GetCellSprite(cell.Effect);
                sr.color = Color.white; 
            }
        }
    }

    private Sprite GetCellSprite(CellEffectType effect)
    {
        switch (effect)
        {
            case CellEffectType.Ice:    return _spriteIce != null ? _spriteIce : _spriteNormal;
            case CellEffectType.Sticky: return _spriteSticky != null ? _spriteSticky : _spriteNormal;
            default:                    return _spriteNormal;
        }
    }
}