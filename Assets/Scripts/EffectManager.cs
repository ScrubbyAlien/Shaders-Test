using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField]
    private CubeGridNavigator cubeGridNavigator;
    [SerializeField]
    private CubeGrid grid;

    [SerializeField]
    private float bobbleTime, bobbleHeight, range, propagationSpeed;
    [SerializeField, Range(0f, 1f)]
    private float dampening;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (cubeGridNavigator.currentCell) {
                StartCoroutine(RippleCircle(cubeGridNavigator.currentCell));
            }
        }
    }

    private IEnumerator RippleCircle(CubeGridCell originCell) {
        List<CubeGridCell> cells = grid.GetCellsWithinRadius(originCell.cellIndexInGrid, range).ToList();
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
