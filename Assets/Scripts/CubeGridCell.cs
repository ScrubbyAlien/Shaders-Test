using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGridCell : MonoBehaviour
{
    private CubeGrid grid;
    public Vector3Int cellIndexInGrid { get; private set; }
    private CubeGridTile tile;

    [SerializeField]
    private MeshRenderer meshRenderer;
    [SerializeField]
    private MeshFilter meshFilter;
    [Header("Materials")]
    [SerializeField]
    private Material highlight;
    [SerializeField]
    private Material noHighlight;

    private Vector3 baseWorldPosition;

    public bool effected { get; private set; }

// private const float halfPi = Mathf.PI * 0.5f;

    public CubeGridCell Initialize(CubeGrid grid, Vector3Int gridPosition, CubeGridTile tile) {
        this.grid = grid;
        this.cellIndexInGrid = gridPosition;
        this.tile = tile;
        meshRenderer.SetSharedMaterials(new() { tile.material, noHighlight });
        // pass cell coord to mesh for use in shader
        Vector2[] uv2 = meshFilter.mesh.uv2;
        for (int i = 0; i < uv2.Length; i++) {
            uv2[i].x = gridPosition.x;
            uv2[i].y = gridPosition.y;
        }
        meshFilter.mesh.uv2 = uv2;
        baseWorldPosition = transform.position;
        return this;
    }

    public Vector3 SurfaceCenter() {
        return transform.position + Vector3.up * (transform.localScale.y * 0.5f);
    }

    public Vector2 XYCenter() {
        Vector3 surfaceCenter = SurfaceCenter();
        return new Vector2(surfaceCenter.x, surfaceCenter.z);
    }

    public void Highlight() {
        meshRenderer.SetSharedMaterials(new() { tile.material, highlight });
    }

    public void Dehighlight() {
        meshRenderer.SetSharedMaterials(new() { tile.material, noHighlight });

    }

    public void Bobble(float time, float height) {
        if (effected) return;
        StartCoroutine(BobbleEffect(time, height));
    }

    private IEnumerator BobbleEffect(float time, float height) {
        effected = true;
        float startTime = Time.time;
        float endtime = startTime + time;
        Vector3 originalPos = transform.position;
        // float maxY = transform.position.y + height;
        // float minY = transform.position.y - height;
        while (Time.time < endtime) {
            float normalizedTime = Mathf.Lerp(0, Mathf.PI, (Time.time - startTime) / time);
            float nextHeight = Mathf.Sin(normalizedTime) * height;
            transform.position = originalPos + new Vector3(0f, nextHeight, 0f);
            yield return null;
        }
        transform.position = baseWorldPosition;
        effected = false;
    }
}
