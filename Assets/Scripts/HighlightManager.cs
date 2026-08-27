using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HighlightManager : MonoBehaviour
{
    [SerializeField]
    private CubeGridNavigator gridNavigator;
    [SerializeField]
    private HighlightMode highlightMode;
    
    private void Start() {
        gridNavigator.NewCellHovered += OnCellHovered;
    }

    private void OnCellHovered(CubeGridCell oldCell, CubeGridCell newCell) {
        switch (highlightMode) {
            case HighlightMode.HighlightHovered:
                oldCell?.Dehighlight();
                newCell?.Highlight();
                break;
            case HighlightMode.NoHighlight:
            default:
                oldCell?.Dehighlight();
                newCell?.Dehighlight();
                break;
        }
    }

    private enum HighlightMode
    {
        HighlightHovered, NoHighlight
    }

    public void DehighlightGroup(IEnumerable<CubeGridCell> cells) {
        foreach (CubeGridCell cell in cells) {
            cell.Dehighlight();
        }
    }
    
    public void HighlightGroup(IEnumerable<CubeGridCell> cells) {
        foreach (CubeGridCell cell in cells) {
            cell.Highlight();
        }
    }
}
