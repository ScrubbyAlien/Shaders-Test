using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "CubeGridTile", menuName = "GridTile")]
public class CubeGridTile : TileBase
{
    public Material material;
    public Color gizmoColor = Color.white;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        tileData.sprite = CreateSpriteFromMaterial(material);
        base.GetTileData(position, tilemap, ref tileData);
    }

    private static Sprite CreateSpriteFromMaterial(Material material) {
        float sideLength = material.mainTexture.width;
        Rect rect = new Rect(0, 0, sideLength, sideLength);
        Sprite sprite = Sprite.Create(material.mainTexture as Texture2D, rect, new Vector2(.5f, .5f), sideLength);
        sprite.name = $"{material.name}_sprite";
        return sprite;
    }
}
