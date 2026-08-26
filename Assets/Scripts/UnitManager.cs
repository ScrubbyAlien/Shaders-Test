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
    private float unitPositionOffset;

    private Transform currentUnit;

    private List<Transform> activeUnits;
    
    private void Start() {
        currentUnit = InstantiateUnit();
        activeUnits = new();
    }

    private void Update() {
        if (!gridNavigator.currentCell) {
            currentUnit.transform.position = Vector3.one * -1000;
            return;
        }

        currentUnit.transform.position = gridNavigator.currentCell.SurfaceCenter() + Vector3.up * unitPositionOffset;

        if (Input.GetMouseButtonDown(0)) {
            activeUnits.Add(currentUnit);
            currentUnit = InstantiateUnit();
        }
    }

    private Transform InstantiateUnit() {
        return Instantiate(unitPrefab, Vector3.one * -1000, unitPrefab.rotation);
    }
}
