using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    private float bobbleTime, bobbleHeight, range, propagationSpeed;
    [SerializeField, Range(0f, 1f)]
    private float dampening;

    [Header("GPUMath")]
    [SerializeField]
    private CubeGrid gridGPUMath;
    [SerializeField]
    private CubeGridNavigator cubeGridNavigatorGPUMath;
    [SerializeField]
    private Material cellRippleMath;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            switch (mode) {
                case Tester.RippleMode.CPUSort:
                    if (cubeGridNavigatorCPU.currentCell) {
                        StartCoroutine(RippleCircle(cubeGridNavigatorCPU.currentCell));
                    }
                    break;
                case Tester.RippleMode.GPUMath:
                    if (cubeGridNavigatorGPUMath.currentCell) {
                        cellRippleMath.SetFloat("StartTime", Time.time);
                        cellRippleMath.SetVector("Origin", cubeGridNavigatorGPUMath.currentCell.SurfaceCenter());
                    }
                    break;
            }
        }
    }

    private IEnumerator RippleCircle(CubeGridCell originCell) {
        List<CubeGridCell> cells = gridCPU.GetCellsWithinRadius(originCell.cellIndexInGrid, range).ToList();
        WaitForSeconds propagationDelay = new WaitForSeconds(propagationSpeed);

        cells.Sort(((cell1, cell2) => {
            float sqrDistanceToOrigin1 = (originCell.XYCenter() - cell1.XYCenter()).sqrMagnitude;
            float sqrDistanceToOrigin2 = (originCell.XYCenter() - cell2.XYCenter()).sqrMagnitude;
            return sqrDistanceToOrigin1.CompareTo(sqrDistanceToOrigin2);
        }));

        float lastDistance = 0f;
        cells[0].Bobble(bobbleTime, bobbleHeight);
        for (int i = 1; i < cells.Count; i++) {
            float distance = (originCell.XYCenter() - cells[i].XYCenter()).sqrMagnitude;
            if (distance != lastDistance) yield return propagationDelay;
            cells[i].Bobble(bobbleTime, bobbleHeight * Mathf.Pow(dampening, distance));
            lastDistance = distance;
        }
    }
}