using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CubeGrid : MonoBehaviour
{
    [SerializeField]
    private CubeGridMap cubeGridMap;

    private void Start() {
        Create();
    }
    
    private void Create() {
        float xOffset = -0.5f + cubeGridMap.mapSize.x / 2f;
        float yOffset = -0.5f + cubeGridMap.mapSize.y / 2f;
        Vector2Int min = cubeGridMap.minPosition;

        foreach (var (tile, pos) in cubeGridMap.AllTilesCompressed()) {
            Vector3 position = new Vector3(pos.x - min.x - xOffset, 0, pos.y - min.y - yOffset);
            GameObject cube = CreateObjectFromTile(position, tile);
        }
        
    }

    private GameObject CreateObjectFromTile(Vector3 position, CubeGridTile tile) {
        GameObject cube = new GameObject("CubeTile");
        cube.AddComponent<MeshFilter>().sharedMesh = tile.mesh;
        cube.AddComponent<MeshRenderer>().sharedMaterial = tile.material;
        cube.transform.position = position;
        return cube;
    }
}