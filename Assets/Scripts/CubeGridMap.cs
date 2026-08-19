using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CubeGridMap : MonoBehaviour
{
    public Tilemap tilemap;

    public Vector2Int mapSize {
        get {
            tilemap.CompressBounds();
            return (Vector2Int)tilemap.cellBounds.size;
        }
    }

    public Vector2Int minPosition {
        get {
            tilemap.CompressBounds();
            return (Vector2Int)tilemap.cellBounds.min;
        }
    }
    
    public IEnumerable<(CubeGridTile, Vector2)> AllTilesCompressed() {
        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++) {
            for (int y = bounds.yMin; y < bounds.yMax; y++) {
                Vector3Int pos = new Vector3Int(x, y);
                CubeGridTile tile = tilemap.GetTile<CubeGridTile>(pos);
                if (!tile) continue;
                yield return (tile, new Vector2(pos.x, pos.y));
            }   
        }
    }

}
