using UnityEngine;

[RequireComponent(typeof(CubeGrid))]
public class CubeGridNavigator : MonoBehaviour
{
    private Camera oCamera;
    private Ray detectCellRay;
    [SerializeField]
    private LayerMask groundLayer;

    private CubeGrid grid;
    private CubeGridCell currentlyHoveredCell;
    
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
            if (currentlyHoveredCell && currentlyHoveredCell == hoveredCell) return; 
            currentlyHoveredCell?.Dehighlight();
            currentlyHoveredCell = hoveredCell;
            currentlyHoveredCell.Highlight();
        }
        else {
            currentlyHoveredCell?.Dehighlight();
            currentlyHoveredCell = null;
        }
    }
}
