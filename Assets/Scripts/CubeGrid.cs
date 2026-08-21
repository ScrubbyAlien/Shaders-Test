using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CubeGrid : MonoBehaviour
{
    [SerializeField]
    private CubeGridMap cubeGridMap;
    [SerializeField]
    private CubeGridCell baseCubeCellPrefab;
    [SerializeField]
    private float heightScale;
    
    private float xOffset => -0.5f + cubeGridMap.mapSize.x / 2f;
    private float yOffset => -0.5f + cubeGridMap.mapSize.y / 2f;
    private Vector2Int min => cubeGridMap.minPosition;

    private CubeGridCell[,] cubeCells;
    
    private void Start() {
        Create();
    }

    private void Create() {
        Vector3Int size = cubeGridMap.tilemap.cellBounds.size;
        Vector3Int min = cubeGridMap.tilemap.cellBounds.min;
        cubeCells = new CubeGridCell[size.x, size.y];
        foreach (var (tile, pos) in cubeGridMap.AllTilesCompressed()) {
            Vector3 worldPosition = GetCenteredPosition(pos);
            Vector3Int cellPosition = pos - min;
            CubeGridCell cell = CreateCellFromTile(worldPosition, cellPosition, tile);
            cubeCells[cellPosition.x, cellPosition.y] = cell;
        }
    }

    private Vector3 GetCenteredPosition(Vector3Int tilemapPosition) {
        return new Vector3(
            tilemapPosition.x - min.x - xOffset, 
            tilemapPosition.z * heightScale, 
            tilemapPosition.y - min.y - yOffset);
    }
    
    private CubeGridCell CreateCellFromTile(Vector3 position, Vector3Int cellPosition, CubeGridTile tile) {
        CubeGridCell cell = Instantiate(baseCubeCellPrefab, transform);
        cell.transform.position = position;
        cell.gameObject.name = $"CubeTile_({cellPosition.x},{cellPosition.y} - h:{cellPosition.z})";
        cell.Initialize(this, cellPosition, tile);
        return cell;
    }

    private void OnDrawGizmos() {
        foreach (var (tile, pos) in cubeGridMap.AllTilesCompressed()) {
            Vector3 position = GetCenteredPosition(pos);
            Gizmos.color = tile.gizmoColor;
            Gizmos.DrawCube(position, Vector3.one * 0.99f);
        }
    }
}