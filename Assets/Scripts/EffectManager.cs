using System;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField]
    private CubeGridNavigator cubeGridNavigator;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (cubeGridNavigator.currentCell) {
                Debug.Log(cubeGridNavigator.currentCell.cellIndexInGrid);
            }
        }
    }
}
