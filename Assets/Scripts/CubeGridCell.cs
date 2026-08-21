using System.Collections.Generic;
using UnityEngine;

public class CubeGridCell : MonoBehaviour
{
    private CubeGrid grid;
    public Vector3Int cellIndexInGrid { get; private set; }
    private CubeGridTile tile;
    
    [SerializeField]
    private MeshRenderer meshRenderer;
    [Header("Materials")]
    [SerializeField]
    private Material highlight;
    [SerializeField]
    private Material noHighlight;

    public CubeGridCell Initialize(CubeGrid grid, Vector3Int gridPosition, CubeGridTile tile) {
        this.grid = grid;
        this.cellIndexInGrid = gridPosition;
        this.tile = tile;
        meshRenderer.SetSharedMaterials(new() { tile.material, noHighlight });
        return this;
    }

    public Vector3 SurfaceCenter() {
        return transform.position + Vector3.up * (transform.localScale.y * 0.5f);
    }

    public void Highlight() {
        meshRenderer.SetSharedMaterials(new() { tile.material, highlight });
    }

    public void Dehighlight() {
        meshRenderer.SetSharedMaterials(new() { tile.material, noHighlight });

    }
}
