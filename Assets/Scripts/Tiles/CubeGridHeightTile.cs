using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "NewHeightTile", menuName = "Height Tile")]
public class CubeGridHeightTile : TileBase
{
    private const int minHeight = -7;
    private const int maxHeight = 8;
    private static float range => maxHeight - minHeight;
    
    [Range(minHeight, maxHeight)]
    public int height;
    [SerializeField]
    private Texture2D numbersTexture;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        tileData.color = Color.white;
        tileData.sprite = GetSpriteFromHeight();
    }

    private Sprite GetSpriteFromHeight() {
        int xIndex = (height % 4) - 1;
        int yIndex = (height - minHeight) / 4;
        Rect rect = new Rect(xIndex * 128, yIndex * 128, 128, 128);
        return Sprite.Create(numbersTexture, rect, Vector2.one * 0.5f, 128);
    }
}
