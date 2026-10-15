using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class Tester : MonoBehaviour
{
    [SerializeField]
    private Vector2Int[] testPositions;
    [SerializeField]
    private float timeBetweenRipples;
    [SerializeField]
    private EffectManager effectManager;
    private WaitForSeconds waitTime;

    private int frames = 0;
    private float testStartTime = 0;

    public enum RippleMode
    {
        CPUSort,
        GPUMath,
        GPUBuffer
    }

    [Button]
    private IEnumerator StartTest() {
        effectManager.autoTesting = false;
        waitTime = new WaitForSeconds(timeBetweenRipples);
        yield return StartCoroutine(TestCPUSort());
        yield return StartCoroutine(TestGPUMath());
        yield return StartCoroutine(TestGPUBuffer());
    }

    private void LateUpdate() {
        frames++;
    }

    private IEnumerator TestCPUSort() {
        // Debug.Log($"Commence test for CPU sort effect");
        effectManager.mode = RippleMode.CPUSort;
        effectManager.SetCorrectGridActive();
        frames = 0;
        testStartTime = Time.time;
        foreach (Vector2Int point in testPositions) {
            effectManager.ExecuteCPUSortEffect(point);
            yield return waitTime;
        }
        float time = Time.time - testStartTime;
        float avgFPS = frames / time;
        Debug.Log($"Results CPU Sort - Total time: {time} | # Frames: {frames} | Average FPS: {avgFPS}");
        // Debug.Log($"Conclude test for CPU sort effect");
    }

    private IEnumerator TestGPUMath() {
        // Debug.Log($"Commence test for GPU math effect");
        effectManager.mode = RippleMode.GPUMath;
        effectManager.SetCorrectGridActive();
        frames = 0;
        testStartTime = Time.time;
        foreach (Vector2Int point in testPositions) {
            effectManager.ExecuteGPUMathEffect(point);
            yield return waitTime;
        }
        float time = Time.time - testStartTime;
        float avgFPS = frames / time;
        Debug.Log($"Results GPU math - Total time: {time} | # Frames: {frames} | Average FPS: {avgFPS}");
        // Debug.Log($"Conclude test for GPU math effect");
    }

    private IEnumerator TestGPUBuffer() {
        // Debug.Log($"Commence test for GPU buffer effect");
        effectManager.mode = RippleMode.GPUBuffer;
        effectManager.SetCorrectGridActive();
        frames = 0;
        testStartTime = Time.time;
        foreach (Vector2Int point in testPositions) {
            effectManager.ExecuteGPUBufferEffect(point);
            yield return waitTime;
        }
        float time = Time.time - testStartTime;
        float avgFPS = frames / time;
        Debug.Log($"Results GPU buffer - Total time: {time} | # Frames: {frames} | Average FPS: {avgFPS}");
        // Debug.Log($"Conclude test for GPU buffer effect");
    }
}