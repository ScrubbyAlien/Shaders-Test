using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CubeGrid))]
public class CubeGridNavigator : MonoBehaviour
{
    public event Action<CubeGridCell, CubeGridCell> NewCellHovered; 
    
    private Camera oCamera;
    private Ray detectCellRay;
    [SerializeField]
    private LayerMask groundLayer;

    private CubeGrid grid;
    private CubeGridCell currentlyHoveredCell;
    public CubeGridCell currentCell => currentlyHoveredCell;
    
    private void Awake() {
        oCamera = Camera.main;
        grid = GetComponent<CubeGrid>();
    }

    private void FixedUpdate() {
        if (!oCamera) return;
        detectCellRay.direction = oCamera.transform.forward;
        detectCellRay.origin = oCamera.ScreenToWorldPoint(Input.mousePosition);
        if (Physics.Raycast(detectCellRay, out RaycastHit hitInfo, 1000f, groundLayer)) {
            CubeGridCell hoveredCell = hitInfo.collider.GetComponent<CubeGridCell>();
            CubeGridCell oldCell = currentlyHoveredCell;
            if (oldCell && oldCell == hoveredCell) return;
            NewCellHovered.Invoke(oldCell, hoveredCell);
            currentlyHoveredCell = hoveredCell;
        }                                     
        else {
            NewCellHovered.Invoke(currentlyHoveredCell, null);
            currentlyHoveredCell = null;
        }
    }

    // private List<CubeGridCell> CalculatePath(Vector3Int start, Vector3Int end) {
    //     
    // }
}                                                                             
