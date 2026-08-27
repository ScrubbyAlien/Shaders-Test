using System;
using System.Collections.Generic;
using System.Linq;
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

    public List<CubeGridCell> CalculatePath(Vector3Int start, Vector3Int end) {
        bool startExists = grid.GetCell(start, out CubeGridCell startCell);
        bool endExists = grid.GetCell(end, out CubeGridCell endCell);
        if (!endExists || !startExists) return new();
        
        Dictionary<CubeGridCell, List<CubeGridCell>> frontier = new() { { startCell, new() { startCell } } };
        Dictionary<CubeGridCell, List<CubeGridCell>> visited = new();

        while (frontier.Count > 0) {
            var (nextCell, nextCellPath) = frontier.First();
            foreach (var (cell, path) in frontier) {
                if (path.Count < nextCellPath.Count) {
                    nextCell = cell;
                    nextCellPath = path;
                }
            }
            
            // move from frontier to visited
            frontier.Remove(nextCell);
            visited.Add(nextCell, nextCellPath);

            // path to end has been found, early exit
            if (nextCell == endCell) {
                return nextCellPath;
            }

            // relax cell
            foreach (CubeGridCell neighbour in grid.GetNeighbours(nextCell.cellIndexInGrid)) {
                if (visited.ContainsKey(neighbour)) continue;
                List<CubeGridCell> pathToNeighbour = new(nextCellPath);
                pathToNeighbour.Add(neighbour);
                if (frontier.TryGetValue(neighbour, out List<CubeGridCell> previousPath)) {
                    if (previousPath.Count > pathToNeighbour.Count) visited[neighbour] = pathToNeighbour;
                }
                else {
                    frontier.Add(neighbour, pathToNeighbour);
                }
            }
        }

        return new();
    }
}                                                                             
