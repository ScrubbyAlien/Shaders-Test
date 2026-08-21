using System.Collections;
using UnityEngine;

public class CameraPivot : MonoBehaviour
{
    [SerializeField]
    private float rotationTime;
    private PivotStates currentState = 0;

    private Coroutine currentlyRotating;

    // TODO: move turn triggering out of this class to some player controller class
    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            Turn(GetNextClockwiseState(currentState));
        }
    }
    
    public void Turn(PivotStates toState) {
        if (currentlyRotating != null) return;
        if (currentState == toState) return;

        Quaternion fromRotation = StateToQuaternion(currentState);
        Quaternion toRotation = StateToQuaternion(toState);
        
        currentlyRotating = StartCoroutine(Rotate(fromRotation, toRotation, rotationTime));

        // not the best place to set the new state, will cause bugs
        currentState = toState;
    }

    private Quaternion StateToQuaternion(PivotStates state) {
        return Quaternion.Euler(new Vector3(0, -90 * (int)state, 0));
    }

    private PivotStates GetNextClockwiseState(PivotStates state) {
        return (PivotStates)(((int)state + 1) % 4);
    }

    private IEnumerator Rotate(Quaternion from, Quaternion to, float time) {
        float startTime = Time.time;

        while (Time.time < startTime + time) {
            float t = (Time.time - startTime) / time;
            transform.rotation = Quaternion.Lerp(from, to, t);
            yield return null;
        }

        transform.rotation = to;
        currentlyRotating = null;
    }
    
    
    public enum PivotStates
    {
        SE = 0, NE = 1, NW = 2, SW = 3 
    }
}
