using NaughtyAttributes;
using UnityEngine;

public class CubeGrid : MonoBehaviour
{
    [SerializeField]
    private Vector2Int size;

    [SerializeField]
    private GameObject cubePrefab;

    [Button]
    private void Create() {
        for (int i = 0; i < transform.childCount; i++) {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                Vector3 position = new Vector3(x, 0, y);
                GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);
                cube.transform.parent = transform;
            }
        }
    }
}