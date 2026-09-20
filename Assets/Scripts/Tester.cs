using System.Collections;
using UnityEngine;

public class Tester : MonoBehaviour
{
    [SerializeField]
    private Vector2Int[] testPositions;
    [SerializeField]
    private float timeBetweenRipples;

    [SerializeField]
    private RippleMode mode;

    public enum RippleMode
    {
        CPUSort,
        GPUMath,
        GPUBuffer
    }

    private void Start() {
        WaitForSeconds waitTime = new WaitForSeconds(timeBetweenRipples);
        StartCoroutine(Test(mode, waitTime, testPositions));
        // seperate coroutine for each mode?
    }

    private void LateUpdate() {
        // measure frame times
        // write to file in csv format
    }

    private IEnumerator Test(RippleMode mode, WaitForSeconds timeBetween, Vector2Int[] ripplePoints) {
        // get effect method based on mode
        Debug.Log($"Commence test for {mode}");
        foreach (Vector2Int point in ripplePoints) {
            // call effect method at point in grid
            Debug.Log(point);
            yield return timeBetween;
        }
        Debug.Log($"Conclude test for {mode}");
    }
}