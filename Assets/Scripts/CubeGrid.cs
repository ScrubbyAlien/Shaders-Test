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
    private float heightScale;
    
    private float xOffset => -0.5f + cubeGridMap.mapSize.x / 2f;
    private float yOffset => -0.5f + cubeGridMap.mapSize.y / 2f;
    private Vector2Int min => cubeGridMap.minPosition;

    private CubeCell[,] cubeCells;
    
    private void Start() {
        Create();
    }
    
    private void Create() {
        Vector3Int size = cubeGridMap.tilemap.cellBounds.size;
        Vector3Int min = cubeGridMap.tilemap.cellBounds.min;
        cubeCells = new CubeCell[size.x, size.y];
        foreach (var (tile, pos) in cubeGridMap.AllTilesCompressed()) {
            Vector3 worldPosition = GetCenteredPosition(pos);
            Vector3Int normalizedCellPosition = pos - min;
            GameObject cube = CreateObjectFromTile(worldPosition, tile);
            CubeCell cell = new CubeCell(cube, pos);
            cubeCells[normalizedCellPosition.x, normalizedCellPosition.y] = cell;
        }
    }

    private Vector3 GetCenteredPosition(Vector3Int tilemapPosition) {
        return new Vector3(
            tilemapPosition.x - min.x - xOffset, 
            tilemapPosition.z * heightScale, 
            tilemapPosition.y - min.y - yOffset);
    }
    
    private GameObject CreateObjectFromTile(Vector3 position, CubeGridTile tile) {
        GameObject cube = new GameObject("CubeTile");
        cube.AddComponent<MeshFilter>().sharedMesh = tile.mesh;
        cube.AddComponent<MeshRenderer>().sharedMaterial = tile.material;
        cube.transform.position = position;
        cube.layer = LayerMask.NameToLayer("ground");
        return cube;
    }

    private void OnDrawGizmos() {
        foreach (var (tile, pos) in cubeGridMap.AllTilesCompressed()) {
            Vector3 position = GetCenteredPosition(pos);
            Gizmos.color = tile.gizmoColor;
            Gizmos.DrawCube(position, Vector3.one * 0.99f);
        }
    }

    private class CubeCell
    {
        public GameObject cubeObject;
        public Vector3Int gridPosition;
        
        public CubeCell(GameObject cubeObject, Vector3Int gridPosition) {
            this.cubeObject = cubeObject;
            this.gridPosition = gridPosition;
        }
    }
}