using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CubeGrid : MonoBehaviour
{
    [SerializeField]
    private CubeGridMap cubeGridMap;
    
    private float xOffset => -0.5f + cubeGridMap.mapSize.x / 2f;
    private float yOffset => -0.5f + cubeGridMap.mapSize.y / 2f;
    private Vector2Int min => cubeGridMap.minPosition;

    private void Start() {
        Create();
    }
    
    private void Create() {
        foreach (var (tile, pos) in cubeGridMap.AllTilesCompressed()) {
            Vector3 position = GetCenteredPosition(pos);
            GameObject cube = CreateObjectFromTile(position, tile);
        }
    }

    private Vector3 GetCenteredPosition(Vector2 tilemapPosition) {
        return new Vector3(tilemapPosition.x - min.x - xOffset, 0, tilemapPosition.y - min.y - yOffset);
    }
    
    private GameObject CreateObjectFromTile(Vector3 position, CubeGridTile tile) {
        GameObject cube = new GameObject("CubeTile");
        cube.AddComponent<MeshFilter>().sharedMesh = tile.mesh;
        cube.AddComponent<MeshRenderer>().sharedMaterial = tile.material;
        cube.transform.position = position;
        return cube;
    }

    private void OnDrawGizmos() {
        foreach (var (tile, pos) in cubeGridMap.AllTilesCompressed()) {
            Vector3 position = GetCenteredPosition(pos);
            Gizmos.color = tile.gizmoColor;
            Gizmos.DrawCube(position, Vector3.one * 0.99f);
        }
    }
}