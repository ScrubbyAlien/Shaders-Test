using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [SerializeField]
    private Transform unitPrefab;
    [SerializeField]
    private CubeGridNavigator gridNavigator;
    [SerializeField]
    private HighlightManager highlightManager;
    [SerializeField]
    private float unitPositionOffset;

    private Transform currentUnit;
    private Vector3Int lastPlacedUnitCoord;
    private List<CubeGridCell> lastHighlightedCells;

    private List<Transform> activeUnits;
    
    private void Start() {
        currentUnit = InstantiateUnit();
        activeUnits = new();
        lastHighlightedCells = new();
        
        // gridNavigator.NewCellHovered += DrawPath;
    }

    private void Update() {
        if (!gridNavigator.currentCell) {
            currentUnit.transform.position = Vector3.one * -1000;
            return;
        }

        currentUnit.transform.position = gridNavigator.currentCell.SurfaceCenter() + Vector3.up * unitPositionOffset;

        if (Input.GetMouseButtonDown(0)) {
            if (activeUnits.Count > 0) return;
            activeUnits.Add(currentUnit);
            lastPlacedUnitCoord = gridNavigator.currentCell.cellIndexInGrid;
            // currentUnit = InstantiateUnit();
        }
    }

    private Transform InstantiateUnit() {
        return Instantiate(unitPrefab, Vector3.one * -1000, unitPrefab.rotation);
    }

    private void DrawPath(CubeGridCell oldCell, CubeGridCell newCell) {
        if (activeUnits.Count == 0) return;
        if (!newCell) return;
        List<CubeGridCell> pathToNewCell = gridNavigator.CalculatePath(lastPlacedUnitCoord, newCell.cellIndexInGrid);
        highlightManager.DehighlightGroup(lastHighlightedCells);
        highlightManager.HighlightGroup(pathToNewCell);
        lastHighlightedCells = pathToNewCell;
    }
}
