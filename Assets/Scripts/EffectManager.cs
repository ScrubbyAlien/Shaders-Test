using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField]
    public Tester.RippleMode mode;

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
    [SerializeField]
    private ComputeShader computeRippleShader;
    private float[] gridData;
    private float[] printData;
    private float[] debugData;
    private ComputeBuffer gridBuffer;
    private ComputeBuffer debugBuffer;
    private Vector3Int groupNums;
    [SerializeField]
    private RippleParams GPUBufferParams;

    public bool autoTesting;

    private void SetDefaultGridData() {
        float[] defaultData = new float[gridGPUBuffer.area];
        for (int i = 0; i < gridBuffer.count; i++) {
            defaultData[i] = -1f;
        }
        gridData = defaultData;
    }

    private void SetNewEffectGridData(Vector2Int coord) {
        SetDefaultGridData();
        gridData[(int)(coord.x + coord.y * gridGPUBuffer.size.x)] = 0f;
    }

    private int MinIntegerMultiple(int divisor, int number) {
        int minMultiple = 0;
        while (minMultiple < number) {
            minMultiple += divisor;
        }
        return minMultiple;
    }

    public void SetCorrectGridActive() {
        switch (mode) {
            case Tester.RippleMode.CPUSort:
                gridCPU.gameObject.SetActive(true);
                gridGPUMath.gameObject.SetActive(false);
                gridGPUBuffer.gameObject.SetActive(false);
                break;
            case Tester.RippleMode.GPUMath:
                gridCPU.gameObject.SetActive(false);
                gridGPUMath.gameObject.SetActive(true);
                gridGPUBuffer.gameObject.SetActive(false);
                break;
            case Tester.RippleMode.GPUBuffer:
                gridCPU.gameObject.SetActive(false);
                gridGPUMath.gameObject.SetActive(false);
                gridGPUBuffer.gameObject.SetActive(true);
                break;
        }
    }

    private void Start() {
        tileGPUMath.material.SetFloat("StartTime", -1000);

        gridBuffer = new ComputeBuffer(gridGPUBuffer.area, sizeof(float));
        debugBuffer = new ComputeBuffer(gridGPUBuffer.area, sizeof(float));
        tileGPUBuffer.material.SetVector("GridSize", gridGPUBuffer.size);
        tileGPUBuffer.material.SetFloat("Amplitude", GPUBufferParams.amplitude);
        computeRippleShader.SetVector("gridSize", gridGPUBuffer.size);
        RippleParams p = GPUBufferParams;
        computeRippleShader.SetFloats("params", p.frequency, p.range, p.propagationSpeed);
        SetDefaultGridData();
        gridBuffer.SetData(gridData);
        tileGPUBuffer.material.SetBuffer("GridBuffer", gridBuffer);
        computeRippleShader.SetBuffer(0, "buffer", gridBuffer);
        computeRippleShader.SetBuffer(0, "debugBuffer", debugBuffer);
        groupNums = GetThreadGroupNumbers(
            computeRippleShader, 0,
            (Vector3Int)gridGPUBuffer.sizeInt
        );
        printData = new float[gridData.Length];
        debugData = new float[gridData.Length];
    }

    private Vector3Int GetThreadGroupNumbers(ComputeShader shader, int kernelIndex, Vector3Int dimensions) {
        shader.GetKernelThreadGroupSizes(0, out uint x, out uint y, out uint z);
        int xThreads = Mathf.CeilToInt(dimensions.x / x) + 1;
        int yThreads = Mathf.CeilToInt(dimensions.y / y) + 1;
        int zThreads = Mathf.CeilToInt(dimensions.z / z) + 1;
        return new Vector3Int(xThreads, yThreads, zThreads);
    }

    private void PrintGridData() {
        string log = "";
        for (int y = 0; y < gridGPUBuffer.size.y; y++) {
            for (int x = 0; x < gridGPUBuffer.size.x; x++) {
                log += $"{printData[(int)(x + y * gridGPUBuffer.size.x)]} ";
            }
            log += "\n";
        }
        Debug.Log(log);
    }

    private void OnDestroy() {
        gridBuffer?.Release();
        debugBuffer?.Release();
    }

    private void Update() {
        if (mode == Tester.RippleMode.GPUBuffer) {
            computeRippleShader.Dispatch(0, groupNums.x, groupNums.y, 1);
        }

        if (autoTesting) return;

        SetCorrectGridActive();

        // debugBuffer.GetData(debugData);
        // Debug.Log(debugData[0]);

        if (Input.GetMouseButtonDown(0)) {
            // gridBuffer.GetData(printData);
            // PrintGridData();

            switch (mode) {
                case Tester.RippleMode.CPUSort:
                    if (cubeGridNavigatorCPU.HasCurrentCell(out CubeGridCell sortCell)) {
                        ExecuteCPUSortEffect(sortCell);
                    }
                    break;
                case Tester.RippleMode.GPUMath:
                    if (cubeGridNavigatorGPUMath.HasCurrentCell(out CubeGridCell mathCell)) {
                        ExecuteGPUMathEffect(mathCell);
                    }
                    break;
                case Tester.RippleMode.GPUBuffer:
                    if (cubeGridNavigatorGPUBuffer.HasCurrentCell(out CubeGridCell bufferCell)) {
                        ExecuteGPUBufferEffect(bufferCell);
                    }
                    break;
            }
        }
    }

    public void ExecuteCPUSortEffect(CubeGridCell cubeCell) {
        StartCoroutine(RippleCircle(cubeCell));
    }
    public void ExecuteCPUSortEffect(Vector2Int coord) {
        gridCPU.GetCell(coord, out CubeGridCell cell);
        ExecuteCPUSortEffect(cell);
    }

    public void ExecuteGPUMathEffect(CubeGridCell cubeCell) {
        tileGPUMath.material.SetFloat("StartTime", Time.time);
        tileGPUMath.material.SetVector("Origin", cubeCell.SurfaceCenter());
    }
    public void ExecuteGPUMathEffect(Vector2Int coord) {
        gridGPUMath.GetCell(coord, out CubeGridCell cell);
        ExecuteGPUMathEffect(cell);
    }

    public void ExecuteGPUBufferEffect(CubeGridCell cubeCell) {
        Vector2Int coord = (Vector2Int)cubeCell.cellIndexInGrid;
        SetNewEffectGridData(coord);
        gridBuffer.SetData(gridData);
        computeRippleShader.SetInts("originCoord", coord.x, coord.y);
    }
    public void ExecuteGPUBufferEffect(Vector2Int coord) {
        gridGPUBuffer.GetCell(coord, out CubeGridCell cell);
        ExecuteGPUBufferEffect(cell);
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