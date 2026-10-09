using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class EffectManager : MonoBehaviour
{
    [SerializeField]
    private Tester.RippleMode mode;

    [Header("CPU")]
    [SerializeField]
    private CubeGrid gridCPU;
    [SerializeField]
    private CubeGridNavigator cubeGridNavigatorCPU;
    [SerializeField]
    private RippleParams CPUSortParams;

    [Header("GPUMath")]
    [SerializeField]
    private CubeGrid gridGPUMath;
    [SerializeField]
    private CubeGridNavigator cubeGridNavigatorGPUMath;
    [SerializeField]
    private CubeGridTile tileGPUMath;

    [Header("GPUBuffer")]
    [SerializeField]
    private CubeGrid gridGPUBuffer;
    [SerializeField]
    private CubeGridNavigator cubeGridNavigatorGPUBuffer;
    [SerializeField]
    private CubeGridTile tileGPUBuffer;
    private float[] gridBuffer;

    private float[] GetDefaultBuffer() {
        for (int i = 0; i < gridBuffer.Length; i++) {
            gridBuffer[i] = -1f;
        }
        return gridBuffer;
    }

    private bool validated;

    private void OnValidate() {
        validated = true;
    }

    private void SetCorrectGridActive() {
        gridCPU.gameObject.SetActive(false);
        gridGPUMath.gameObject.SetActive(false);
        gridGPUBuffer.gameObject.SetActive(false);

        CubeGrid activeGrid = mode switch {
            Tester.RippleMode.CPUSort => gridCPU,
            Tester.RippleMode.GPUMath => gridGPUMath,
            Tester.RippleMode.GPUBuffer => gridGPUBuffer,
            _ => gridCPU
        };

        activeGrid.gameObject.SetActive(true);
    }

    private void Start() {
        gridBuffer = new float[(int)(gridGPUBuffer.size.x * gridGPUBuffer.size.y)];
        tileGPUMath.material.SetFloat("StartTime", -1000);
        tileGPUBuffer.material.SetVector("GridSize", gridGPUBuffer.size);
        tileGPUBuffer.material.SetFloatArray("GridBuffer", GetDefaultBuffer());
    }

    private void Update() {
        if (validated) {
            SetCorrectGridActive();
            validated = false;
        }
        if (Input.GetMouseButtonDown(0)) {
            switch (mode) {
                case Tester.RippleMode.CPUSort:
                    if (cubeGridNavigatorCPU.currentCell) {
                        StartCoroutine(RippleCircle(cubeGridNavigatorCPU.currentCell));
                    }
                    break;
                case Tester.RippleMode.GPUMath:
                    if (cubeGridNavigatorGPUMath.currentCell) {
                        tileGPUMath.material.SetFloat("StartTime", Time.time);
                        tileGPUMath.material.SetVector("Origin", cubeGridNavigatorGPUMath.currentCell.SurfaceCenter());
                    }
                    break;
                case Tester.RippleMode.GPUBuffer:
                    if (cubeGridNavigatorGPUBuffer.currentCell) {
                        // clear buffer and set origin
                    }
                    break;
            }
        }
    }

    private IEnumerator RippleCircle(CubeGridCell originCell) {
        List<CubeGridCell> cells = gridCPU.GetCellsWithinRadius(originCell.cellIndexInGrid, CPUSortParams.range)
                                          .ToList();

        cells.Sort(((cell1, cell2) => {
            float sqrDistanceToOrigin1 = (originCell.XYCenter() - cell1.XYCenter()).sqrMagnitude;
            float sqrDistanceToOrigin2 = (originCell.XYCenter() - cell2.XYCenter()).sqrMagnitude;
            return sqrDistanceToOrigin1.CompareTo(sqrDistanceToOrigin2);
        }));

        float lastDistance = 0f;
        float bobbleTime = 1 / (CPUSortParams.frequency * Mathf.PI);
        cells[0].Bobble(bobbleTime, CPUSortParams.amplitude);
        for (int i = 1; i < cells.Count; i++) {
            float distance = (originCell.XYCenter() - cells[i].XYCenter()).sqrMagnitude;
            if (distance != lastDistance) {
                float difference = distance - lastDistance;
                yield return new WaitForSeconds(difference / (CPUSortParams.propagationSpeed * Mathf.PI));
            }
            cells[i].Bobble(bobbleTime, CPUSortParams.amplitude);
            lastDistance = distance;
        }
    }

    [Serializable]
    private struct RippleParams
    {
        public float amplitude;
        public float frequency;
        public float range;
        public float propagationSpeed;
    }
}